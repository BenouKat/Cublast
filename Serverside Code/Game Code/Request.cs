using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace Cublast
{
    public class Request
    {
        public string requestType;
        public string fromUser;
        public string toUser;
        public string content;
        public DateTime date;

        public Request(string requestType, string fromUser, string toUser, string content)
        {
            this.requestType = requestType;
            this.fromUser = fromUser;
            this.toUser = toUser;
            this.content = content;
            date = DateTime.UtcNow;
        }

        public Request(DatabaseObject requestDbo)
        {
            requestType = requestDbo.GetString("Type");
            fromUser = requestDbo.GetString("FromUser");
            toUser = requestDbo.GetString("ToUser");
            content = requestDbo.GetString("Content");
            date = requestDbo.GetDateTime("Date");
        }

        public DatabaseObject toDbo()
        {
            DatabaseObject dbo = new DatabaseObject();
            dbo.Set("Type", requestType);
            dbo.Set("FromUser", fromUser);
            dbo.Set("ToUser", toUser);
            dbo.Set("Content", content);
            dbo.Set("Date", date);
            return dbo;
        }
    }
}
