using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace Cublast
{
    public class User
    {
        public DateTime dateCreation;
        public DateTime lastConnection;
        public DateTime lastDisconnection;
        public int playTime;

        public int songCleared;

        public string currentSongPlayed;

        public string lastSongPlayed;
        public float lastScore;

        public User()
        {
            dateCreation = DateTime.UtcNow;
            lastConnection = DateTime.UtcNow;
            lastDisconnection = DateTime.UtcNow.AddHours(-1);
            songCleared = 0;
            currentSongPlayed = "";
            lastSongPlayed = "";
            lastScore = 0f;
        }

        public User(DatabaseObject userDbo)
        {
            dateCreation = userDbo.GetDateTime("DateCreation");
            lastConnection = userDbo.GetDateTime("LastConnection");
            lastDisconnection = userDbo.GetDateTime("LastDisconnection");
            playTime = userDbo.GetInt("PlayTime");

            songCleared = userDbo.GetInt("SongCleared");
            currentSongPlayed = userDbo.GetString("CurrentSongPlayed");
            lastSongPlayed = userDbo.GetString("LastSongPlayed");
            lastScore = userDbo.GetFloat("LastScore");
        }

        public DatabaseObject toDbo()
        {
            DatabaseObject dbo = new DatabaseObject();
            dbo.Set("DateCreation", dateCreation);
            dbo.Set("LastConnection", lastConnection);
            dbo.Set("LastDisconnection", lastDisconnection);
            dbo.Set("PlayTime", playTime);
            dbo.Set("SongCleared", songCleared);
            dbo.Set("CurrentSongPlayed", currentSongPlayed);
            dbo.Set("LastSongPlayed", lastSongPlayed);
            dbo.Set("LastScore", lastScore);
            return dbo;
        }

        public void updateConnection(ref DatabaseObject userDbo)
        {
            lastConnection = DateTime.UtcNow;
            userDbo.Set("LastConnection", lastConnection);
        }

        public void updateDisconnection(ref DatabaseObject userDbo)
        {
            lastDisconnection = DateTime.UtcNow;
            playTime += (int)(lastDisconnection - lastConnection).TotalSeconds;
            userDbo.Set("LastDisconnection", lastDisconnection);
            userDbo.Set("PlayTime", playTime);
        }

        public void updateSongPlayed(string songPlayed, int level)
        {

        }

        public void updateLastSongPlayed()
        {

        }
    }
}
