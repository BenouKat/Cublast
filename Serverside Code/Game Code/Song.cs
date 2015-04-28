using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace Cublast
{
    public class Song
    {
        public string worldRecord;
        public int worldRecordScore;

        public Song()
        {
            worldRecord = "";
            worldRecordScore = 0;
        }

        public Song(DatabaseObject songDbo)
        {
            worldRecord = songDbo.GetString("WorldRecord");
            worldRecordScore = songDbo.GetInt("WorldRecordScore");
        }

        public DatabaseObject toDbo()
        {
            DatabaseObject dbo = new DatabaseObject();
            dbo.Set("WorldRecord", worldRecord);
            dbo.Set("WorldRecordScore", worldRecordScore);
            return dbo;
        }

        public void updateWorldRecord(ref DatabaseObject songDbo, string name, int score)
        {
            worldRecord = name;
            worldRecordScore = score;
            songDbo.Set("WorldRecord", name);
            songDbo.Set("WorldRecordScore", score);
        }
    }
}
