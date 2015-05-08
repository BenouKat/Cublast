using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Security;
using System.Security.Cryptography;
using PlayerIO.GameLibrary;

namespace Cublast {

	[RoomType("CublastSolo")]
	public class SoloGameCode : Game<Player> {

		// This method is called when an instance of your the game is created
        public override void GameStarted()
        {
            // anything you write to the Console will show up in the 
            // output window of the development server
            Console.WriteLine("Game is started: " + RoomId);
        }

		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed() {
			Console.WriteLine("RoomId: " + RoomId);
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player player) {

            loadUserObject(player.userId, delegate(DatabaseObject userDbo)
            {
                if (userDbo == null)
                {
                    //Creation
                    User u = new User();
                    Pack p = new Pack();
                    Friendship f = new Friendship();

                    PlayerIO.BigDB.CreateObject("Users", player.userId, u.toDbo(), delegate(DatabaseObject createdUserDbo)
                    {
                        player.user = new User(createdUserDbo);
                        PlayerIO.BigDB.CreateObject("Packs", player.userId, p.toDbo(), delegate(DatabaseObject createdPackDbo)
                        {
                            player.packs = new Pack(createdPackDbo);
                            PlayerIO.BigDB.CreateObject("Friendships", player.userId, f.toDbo(), delegate(DatabaseObject createdFriendshipDbo)
                            {
                                player.friends = new Friendship(createdFriendshipDbo);
                                player.Send("initializeDataSuccess");
                            });
                        });
                    });
                }
                else
                {
                    player.user = new User(userDbo);

                    //Updating connection time
                    player.user.updateConnection(ref userDbo);
                    userDbo.Save();

                    //load packs
                    loadPacksObject(player.userId, delegate(DatabaseObject packDbo)
                    {
                        Pack p = new Pack(packDbo);
                        player.packs = p;
                        player.Send("initializeDataSuccess");
                    });

                    loadFriendshipObject(player.userId, delegate(DatabaseObject friendshipDbo)
                    {
                        Friendship f = new Friendship(friendshipDbo);
                        player.friends = f;
                    });
                }
                
            },
            delegate(PlayerIOError error)
            {
                Console.WriteLine(error.Message);
            });
		}

		// This method is called when a player leaves the game
		public override void UserLeft(Player player) {

            //Updating disconnection
            loadUserObject(player.userId, delegate(DatabaseObject userDbo)
            {
                player.user.updateDisconnection(ref userDbo);
                player.user.updateSongPlayed(ref userDbo, "", 0);
                userDbo.Save();
            });
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(Player player, Message message) {
			switch(message.Type) {

                case "SongCleared":
                    processSongCleared(player, message.GetString(0), message.GetString(1), message.GetInt(2), message.GetString(3));
                    break;
                case "UpdateCurrentSong":
                    updateCurrentSong(player, message.GetString(0), message.GetInt(1));
                    break;
                case "UpdatePacks":
                    updatePacks(player, message.GetString(0));
                    break;
                case "SendRequest":
                    sendRequest(player, message.GetString(0), message.GetString(1), message.GetString(2));
                    break;
                case "AcceptFriend":
                    break;
			}
		}


        //Generic methods
        public void loadUserObject(string userId, Callback<DatabaseObject> success = null, Callback<PlayerIOError> failure = null)
        {
            PlayerIO.BigDB.Load("Users", userId, delegate(DatabaseObject userDbo)
            {
                if (success != null) success(userDbo);
            }, delegate(PlayerIOError error)
            {
                if (failure != null) failure(error);
            });
        }

        public void loadSongObject(string songId, Callback<DatabaseObject> success = null, Callback<PlayerIOError> failure = null)
        {
            PlayerIO.BigDB.Load("Songs", songId, delegate(DatabaseObject songDbo)
            {
                if (success != null) success(songDbo);
            }, delegate(PlayerIOError error)
            {
                if (failure != null) failure(error);
            });
        }

        public void loadSongTopObject(string songId, Callback<DatabaseObject> success = null, Callback<PlayerIOError> failure = null)
        {
            PlayerIO.BigDB.Load("SongsTop", songId, delegate(DatabaseObject songTopDbo)
            {
                if (success != null) success(songTopDbo);
            }, delegate(PlayerIOError error)
            {
                if (failure != null) failure(error);
            });
        }

        public void loadPacksObject(string userId, Callback<DatabaseObject> success = null, Callback<PlayerIOError> failure = null)
        {
            PlayerIO.BigDB.Load("Packs", userId, delegate(DatabaseObject packDbo)
            {
                if (success != null) success(packDbo);
            }, delegate(PlayerIOError error)
            {
                if (failure != null) failure(error);
            });
        }

        public void loadFriendshipObject(string userId, Callback<DatabaseObject> success = null, Callback<PlayerIOError> failure = null)
        {
            PlayerIO.BigDB.Load("Friendships", userId, delegate(DatabaseObject friendshipDbo)
            {
                if (success != null) success(friendshipDbo);
            }, delegate(PlayerIOError error)
            {
                if (failure != null) failure(error);
            });
        }

        public void findRequestObject(string userId, Callback<DatabaseObject> success = null, Callback<PlayerIOError> failure = null)
        {
            PlayerIO.BigDB.LoadRange("Requests", "byFromUser", null, userId, userId, 1, delegate(DatabaseObject[] requestDbo)
            {
                if (success != null)
                {
                    if (requestDbo != null && requestDbo.Length > 0)
                    {
                        success(requestDbo[0]);
                    }
                    else
                    {
                        success(null);
                    }
                } 
            }, delegate(PlayerIOError error)
            {
                if (failure != null) failure(error);
            });
        }



        //Game method
        public void processSongCleared(Player player, string songIdentifier, string songName, int level, string scoreCrypted)
        {

            //Decryption
            int startSubstring = (songIdentifier.Length / 2) - 10;
            if(startSubstring < 0) startSubstring = 0;
            int endSubstring = 20;
            if (endSubstring > songIdentifier.Length - startSubstring) endSubstring = songIdentifier.Length - startSubstring;
            string identifier = songIdentifier.Substring(startSubstring, endSubstring);

            String scoreDecrypted = "";
            MD5CryptoServiceProvider hashMd5 = new MD5CryptoServiceProvider();
            byte[] passwordHash = hashMd5.ComputeHash(
            UnicodeEncoding.Unicode.GetBytes(identifier));

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = passwordHash;
            des.Mode = CipherMode.ECB;

            byte[] buffer = UnicodeEncoding.Unicode.GetBytes(scoreCrypted);
            scoreDecrypted = UnicodeEncoding.Unicode.GetString(
            des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));


            double score = double.Parse(scoreDecrypted);

            //Update the last song played and null the song played
            loadUserObject(player.userId, delegate(DatabaseObject userDbo)
            {
                player.user.updateSongPlayed(ref userDbo, songName, level);
                player.user.updateLastSongPlayed(ref userDbo, songName, level, (float)score);
                userDbo.Save();
            });

            int scoreTransformed = (int)(score * 100);
            checkForWorldRecord(player, songIdentifier, scoreTransformed);
            checkForSongTop(player, songIdentifier, scoreTransformed);
        }

        public void checkForWorldRecord(Player player, string songIdentifier, int scoreTransformed)
        {
            //Check for world record
            loadSongObject(songIdentifier, delegate(DatabaseObject songDbo)
            {
                //Song is not recorded
                if (songDbo == null)
                {
                    createSongRecord(player, songIdentifier, scoreTransformed);
                    return;
                }

                Song s = new Song(songDbo);

                //If the score is equal or better
                if (s.worldRecordScore <= scoreTransformed)
                {
                    //If he doesn't have the 
                    if (!s.worldRecord.Equals(player.userId))
                    {
                        updateUserWorldRecord(player, player.userId, 1);

                        if (!string.IsNullOrEmpty(s.worldRecord))
                        {
                            updateUserWorldRecord(player, s.worldRecord, -1);
                        }

                        displayANews(player, "WorldRecord", player.userId, scoreTransformed);

                    }

                    updateSongWorldRecord(songIdentifier, player.userId, scoreTransformed);
                }

            },

            delegate(PlayerIOError error)
            {
                Console.WriteLine(error.Message);
            });
        }

        public void checkForSongTop(Player player, string songIdentifier, int scoreTransformed)
        {
            loadSongTopObject(songIdentifier, delegate(DatabaseObject songTopDbo)
            {
                SongTop top = new SongTop(songTopDbo);

                if (top.isEligibleForTop(player.userId, scoreTransformed))
                {
                    top.insertToSongTop(player.userId, scoreTransformed);
                    top.updateRanking(ref songTopDbo);
                    songTopDbo.Save(true, delegate()
                    {
                        //Ok !
                    }, delegate(PlayerIOError error)
                    {
                        if (error.ErrorCode == ErrorCode.StaleVersion)
                        {
                            checkForSongTop(player, songIdentifier, scoreTransformed);
                        }
                    });
                }

            },

            delegate(PlayerIOError error)
            {
                Console.WriteLine(error.Message);
            });
        }

        public void createSongRecord(Player player, string songIdentifier, int scoreTransformed)
        {
            Song s = new Song();
            s.worldRecord = player.userId;
            s.worldRecordScore = scoreTransformed;
           
            PlayerIO.BigDB.CreateObject("Songs", songIdentifier, s.toDbo(), delegate(DatabaseObject songDbo)
            {
                updateUserWorldRecord(player, player.userId, 1);
                displayANews(player, "WorldRecord", player.userId, scoreTransformed);
            });

            SongTop stop = new SongTop();
            stop.users.Add(player.userId);
            stop.score.Add(scoreTransformed);

            PlayerIO.BigDB.CreateObject("SongsTop", songIdentifier, stop.toDbo(), delegate(DatabaseObject songTopDbo)
            {
                //Ok !
            });
        }

        public void updateUserWorldRecord(Player player, string userId, int count)
        {
            loadUserObject(userId, delegate(DatabaseObject userDbo)
            {
                (userId.Equals(player.userId) ? player.user : new User(userDbo)).updateWorldRecord(ref userDbo, count);
                userDbo.Save(true, delegate()
                {
                    //Ok !
                }, delegate(PlayerIOError error)
                {
                    if (error.ErrorCode == ErrorCode.StaleVersion)
                    {
                        updateUserWorldRecord(player, userId, count);
                    }
                });
            });
        }

        public void updateSongWorldRecord(string songIdentifier, string name, int score)
        {
            loadSongObject(songIdentifier, delegate(DatabaseObject songDbo)
            {
                Song s = new Song(songDbo);
                s.updateWorldRecord(ref songDbo, name, score);
                songDbo.Save(true, delegate()
                {
                    //Ok !   
                }, delegate(PlayerIOError error)
                {
                    if (error.ErrorCode == ErrorCode.StaleVersion)
                    {
                        updateSongWorldRecord(songIdentifier, name, score);
                    }
                });
            });
        }

        //Update the current song played
        public void updateCurrentSong(Player player, string songName, int level)
        {
            loadUserObject(player.userId, delegate(DatabaseObject userDbo)
            {
                player.user.updateSongPlayed(ref userDbo, songName, level);
                userDbo.Save();
            });
        }


        //Update available packs
        public void updatePacks(Player player, string packs)
        {
            loadPacksObject(player.userId, delegate(DatabaseObject packDbo)
            {
                string[] packsSplit = packs.Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries);
                player.packs.updatePackArray(ref packDbo, packsSplit);
                packDbo.Save();
            });
        }

        public void displayANews(Player player, string newsType, string name, int score)
        {
            News news = new News(newsType, name, score);
            PlayerIO.BigDB.CreateObject("News", null, news.toDbo(), delegate(DatabaseObject dbo)
            {
                PlayerIO.BigDB.LoadRange("News", "byDate", null, null, DateTime.Now, 100, delegate(DatabaseObject[] allNewsDbo)
                {
                    if (allNewsDbo != null)
                    {
                        List<News> allNews = new List<News>();
                        foreach (DatabaseObject newsDbo in allNewsDbo)
                        {
                            allNews.Add(new News(newsDbo));
                        }
                        allNews.Sort(delegate(News x, News y)
                        {
                            return DateTime.Compare(x.date, y.date);
                        });

                        if (allNews.Count > 10)
                        {
                            List<string> keys = new List<string>();
                            for (int i = 0; i < allNews.Count - 10; i++)
                            {
                                keys.Add(allNews[i].key);
                            }
                            PlayerIO.BigDB.DeleteKeys("News", keys.ToArray());
                        }
                    }
                });
            });
            
        }

        public void sendRequest(Player player, string requestType, string toUser, string content)
        {
            Request request = new Request(requestType, player.userId, toUser, content);
            PlayerIO.BigDB.CreateObject("Requests", null, request.toDbo(), delegate(DatabaseObject requestSuccess)
            {
                //Ok !
            });
        }

        
        //Friends ships
        public void acceptFriend(Player player, string userAsking)
        {
            findRequestObject(userAsking, delegate(DatabaseObject requestDbo)
            {
                addFriend(player, player.userId, userAsking);
                addFriend(player, userAsking, player.userId);

                PlayerIO.BigDB.DeleteKeys("Requests", requestDbo.Key);
            });
        }

        public void removeFriend(Player player, string userAsking)
        {
            deleteFriend(player, player.userId, userAsking);
            deleteFriend(player, userAsking, player.userId);
        }

        public void addFriend(Player player, string userId, string friend)
        {
            loadFriendshipObject(userId, delegate(DatabaseObject friendships)
            {
                Friendship f = player.userId.Equals(userId) ? player.friends : new Friendship(friendships);
                if (!f.friends.Contains(friend))
                {
                    f.friends.Add(friend);
                }

                f.updateFriend(ref friendships);
                friendships.Save(true, delegate()
                {
                    //Ok !
                }, delegate(PlayerIOError error)
                {
                    if (error.ErrorCode == ErrorCode.StaleVersion)
                    {
                        addFriend(player, userId, friend);
                    }
                });
            });
        }

        public void deleteFriend(Player player, string userId, string friend)
        {
            loadFriendshipObject(userId, delegate(DatabaseObject friendships)
            {
                Friendship f = player.userId.Equals(userId) ? player.friends : new Friendship(friendships);
                if (f.friends.Contains(friend))
                {
                    f.friends.Remove(friend);
                }

                f.updateFriend(ref friendships);
                friendships.Save(true, delegate()
                {
                    //Ok !
                }, delegate(PlayerIOError error)
                {
                    if (error.ErrorCode == ErrorCode.StaleVersion)
                    {
                        deleteFriend(player, userId, friend);
                    }
                });
            });
        }
	}
}