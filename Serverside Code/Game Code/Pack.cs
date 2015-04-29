using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace Cublast
{
    public class Pack
    {
        public List<string> packName;

        public Pack()
        {
            packName = new List<string>();
        }

        public Pack(DatabaseObject packDbo)
        {
            packName = new List<string>();
            DatabaseArray array = packDbo.GetArray("PackList");
            for (int i = 0; i < array.Count; i++ )
            {
                packName.Add(array.GetString(i));
            }
        }

        public DatabaseObject toDbo()
        {
            DatabaseObject dbo = new DatabaseObject();

            DatabaseArray array = new DatabaseArray();
            for (int i = 0; i < packName.Count; i++)
            {
                array.Add(packName[0]);
            }

            dbo.Set("PackList", array);
            return dbo;
        }

        public void updatePackArray(ref DatabaseObject dbo, string[] packArray)
        {
            packName.Clear();
            packName.AddRange(packArray);

            DatabaseArray array = new DatabaseArray();
            for (int i = 0; i < packName.Count; i++)
            {
                array.Add(packName[0]);
            }

            dbo.Set("PackList", array);
        }
    }
}
