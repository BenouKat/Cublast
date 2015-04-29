using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
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
                    PlayerIO.BigDB.CreateObject("Users", player.userId, u.toDbo(), delegate(DatabaseObject createdUserDbo)
                    {
                        player.user = new User(createdUserDbo);
                        PlayerIO.BigDB.CreateObject("Packs", player.userId, p.toDbo(), delegate(DatabaseObject createdPackDbo)
                        {
                            player.packs = new Pack(createdPackDbo);
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
                    processSongCleared(player, message.GetString(0), message.GetString(1), message.GetInt(2), message.GetDouble(3));
                    break;
                case "UpdateCurrentSong":
                    updateCurrentSong(player, message.GetString(0), message.GetInt(1));
                    break;
                case "UpdatePacks":
                    updatePacks(player, message.GetString(0));
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



        //Game method
        public void processSongCleared(Player player, string songIdentifier, string songName, int level, double score)
        {
            //Update the last song played and null the song played
            loadUserObject(player.userId, delegate(DatabaseObject userDbo)
            {
                player.user.updateSongPlayed(ref userDbo, songName, level);
                player.user.updateLastSongPlayed(ref userDbo, songName, level, (float)score);
                userDbo.Save();
            });

            //Check for world record
            int scoreTransformed = (int)(score * 100);
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
                        loadUserObject(player.userId, delegate(DatabaseObject newRecordmanDbo)
                        {
                            player.user.updateWorldRecord(ref newRecordmanDbo, 1);
                        });

                        if (!string.IsNullOrEmpty(s.worldRecord))
                        {
                            loadUserObject(s.worldRecord, delegate(DatabaseObject oldRecordmanDbo)
                            {
                                User oldRecord = new User(oldRecordmanDbo);
                                oldRecord.updateWorldRecord(ref oldRecordmanDbo, -1);
                            });
                        }

                        displayANews(player, "WorldRecord", player.userId, scoreTransformed);

                    }

                    s.updateWorldRecord(ref songDbo, player.userId, scoreTransformed);
                    songDbo.Save();
                }


                //Vérifier s'il est éligible pour le top 100
                //Si oui 

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
                loadUserObject(player.userId, delegate(DatabaseObject userDbo)
                {
                    player.user.updateWorldRecord(ref userDbo, 1);
                });

                displayANews(player, "WorldRecord", player.userId, scoreTransformed);
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
                string[] packsSplit = packs.Split('|');
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
	}
}