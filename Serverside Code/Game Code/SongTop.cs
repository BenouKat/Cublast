using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace Cublast
{
    public class SongTop
    {
        public List<string> users;
        public List<int> score;

        public SongTop()
        {

        }

        public SongTop(DatabaseObject songTopDbo)
        {
            users = new List<string>();
            score = new List<int>();

            DatabaseArray arrayTop = songTopDbo.GetArray("Ranking");
            for (int i = 0; i < arrayTop.Count; i++)
            {
                DatabaseObject rankDbo = arrayTop.GetObject(i);
                users.Add(rankDbo.GetString("User"));
                score.Add(rankDbo.GetInt("Score"));
            }
        }

        public void updateRanking(ref DatabaseObject songTopDbo)
        {
            DatabaseArray arrayTop = new DatabaseArray();
            for (int i = 0; i < users.Count; i++)
            {
                DatabaseObject rankDbo = new DatabaseObject();
                rankDbo.Set("User", users[i]);
                rankDbo.Set("Score", score[i]);
                arrayTop.Add(rankDbo);
            }
            songTopDbo.Set("Ranking", arrayTop);
        }

        public bool isEligibleForTop(string user, int userScore)
        {
            if (users.Exists(c => c.Equals(user)))
            {
                int index = users.IndexOf(user);
                if(userScore < score[index]) return false;
            }

            if (users.Count >= 100 && score[99] > userScore)
            {
                return false;
            }

            return true;
        }

        public void insertToSongTop(string user, int userScore)
        {
            if(users.Exists(c => c.Equals(user)))
            {
                int index = users.IndexOf(user);
                users.RemoveAt(index);
                score.RemoveAt(index);
            }

            for (int i = 0; i < users.Count; i++)
            {
                if (score[i] <= userScore)
                {
                    users.Insert(i, user);
                    score.Insert(i, userScore);

                    if (users.Count > 100)
                    {
                        for (int j = users.Count-1; j>99 ; j--)
                        {
                            users.RemoveAt(j);
                            score.RemoveAt(j);
                        }
                    }
                }
            }
        }
    }
}
