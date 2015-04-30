using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace Cublast
{
    public class Friendship
    {
        public List<string> friends;

        public Friendship()
        {
            friends = new List<string>();
        }

        public Friendship(DatabaseObject dbo)
        {
            friends = new List<string>();

            DatabaseArray array = dbo.GetArray("Friends");
            for (int i = 0; i < array.Count; i++)
            {
                friends.Add(array.GetString(i));
            }
        }

        public DatabaseObject toDbo()
        {
            DatabaseObject dbo = new DatabaseObject();
            updateFriend(ref dbo);
            return dbo;
        }

        public void updateFriend(ref DatabaseObject friendDbo)
        {
            DatabaseArray arrayDbo = new DatabaseArray();
            foreach (string user in friends)
            {
                arrayDbo.Add(user);
            }
            friendDbo.Set("Friends", arrayDbo);
        }
    }
}
