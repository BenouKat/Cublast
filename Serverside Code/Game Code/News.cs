using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace Cublast
{
    public class News
    {
        public string key;
        public string newsType;
        public string name;
        public int score;
        public DateTime date;

        public News(string newsType, string name, int score)
        {
            this.newsType = newsType;
            this.name = name;
            this.score = score;
            date = DateTime.Now;
        }

        public News(DatabaseObject newsDbo)
        {
            key = newsDbo.Key;
            newsType = newsDbo.GetString("NewsType");
            name = newsDbo.GetString("Name");
            score = newsDbo.GetInt("Score");
            date = newsDbo.GetDateTime("Date");
        }

        public DatabaseObject toDbo()
        {
            DatabaseObject dbo = new DatabaseObject();
            dbo.Set("NewsType", newsType);
            dbo.Set("Name", name);
            dbo.Set("Score", score);
            dbo.Set("Date", date);

            return dbo;
        }
    }
}
