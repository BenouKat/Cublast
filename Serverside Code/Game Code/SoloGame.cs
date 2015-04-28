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

            loadUser(player.userId, delegate(User u)
            {
                player.user = u;
                //Updating connection time
                loadUserObject(player.userId, delegate(DatabaseObject userDbo)
                {
                    player.user.updateConnection(ref userDbo);
                    userDbo.Save();
                });
            },
            delegate()
            {
                //Creation
                User u = new User();
                PlayerIO.BigDB.CreateObject("Users", player.userId, u.toDbo(), delegate(DatabaseObject userDbo)
                {
                    player.user = new User(userDbo);
                });

            });
		}

		// This method is called when a player leaves the game
		public override void UserLeft(Player player) {

            //Updating disconnection
            loadUserObject(player.userId, delegate(DatabaseObject userDbo)
            {
                player.user.updateDisconnection(ref userDbo);
                userDbo.Save();
            });
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(Player player, Message message) {
			switch(message.Type) {

                case "SongCleared":
                    processSongCleared(player, message.GetString(0), message.GetString(1), message.GetInt(2), message.GetDouble(3));
                    break;
                case "UpdateCurrentSong"
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

        public void loadUser(string userId, Callback<User> success = null, Callback failure = null)
        {
            loadUserObject(userId, delegate(DatabaseObject userDbo)
            {
                User user = new User(userDbo);
                if (success != null) success(user);
            },
            delegate(PlayerIOError error)
            {
                if (failure != null) failure();
            });
        }

        public void loadSongObject(string songId, Callback<DatabaseObject> success = null, Callback<PlayerIOError> failure = null)
        {
            PlayerIO.BigDB.Load("Songs", songId, delegate(DatabaseObject userDbo)
            {
                if (success != null) success(userDbo);
            }, delegate(PlayerIOError error)
            {
                if (failure != null) failure(error);
            });
        }




        //Game method
        public void processSongCleared(Player player, string songIdentifier, string songName, int level, double score)
        {
            loadUserObject(player.userId, delegate(DatabaseObject userDbo)
            {
                player.user.updateSongPlayed(ref userDbo, songName, level);
                player.user.updateLastSongPlayed(ref userDbo, songName, level, (float)score);
                userDbo.Save();
            });

            int scoreTransformed = (int)(score * 100);
            loadSongObject(songIdentifier, delegate(DatabaseObject songDbo)
            {
                Song s = new Song(songDbo);
                
                if (s.worldRecordScore <= scoreTransformed)
                {
                    loadUserObject(player.userId, delegate(DatabaseObject userDbo)
                    {
                        player.user.updateWorldRecord(ref userDbo, 1);
                    });
                    s.updateWorldRecord(ref songDbo, player.userId, scoreTransformed);
                    songDbo.Save();
                }
            },
            delegate(PlayerIOError error)
            {
                createSongRecord(player, songIdentifier, scoreTransformed);
                loadUserObject(player.userId, delegate(DatabaseObject userDbo)
                {
                    player.user.updateWorldRecord(ref userDbo, 1);
                });
            });
        }

        public void createSongRecord(Player player, string songIdentifier, int scoreTransformed)
        {
            Song s = new Song();
            s.worldRecord = player.userId;
            s.worldRecordScore = scoreTransformed;
            PlayerIO.BigDB.CreateObject("Songs", songIdentifier, s.toDbo(), delegate(DatabaseObject songDbo)
            {
                //Ok !
            });
        }

        public void updateCurrentSong(Player player, string songName, int level)
        {
            loadUserObject(player.userId, delegate(DatabaseObject userDbo)
            {
                player.user.updateSongPlayed(ref userDbo, songName, level);
                userDbo.Save();
            });
        }
	}
}