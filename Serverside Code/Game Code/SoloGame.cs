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

                

				case "Chat":
					foreach(Player pl in Players) {
						if(pl.ConnectUserId != player.ConnectUserId) {
							pl.Send("Chat", player.ConnectUserId, message.GetString(0));
						}
					}
					break;
			}
		}



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
	}
}