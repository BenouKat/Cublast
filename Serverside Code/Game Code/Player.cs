using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace Cublast
{
    public class Player : BasePlayer
    {
        public string userId { 
            get{
                if (this.ConnectUserId.Contains("simple"))
                {
                    return this.ConnectUserId.Substring(6, this.ConnectUserId.Length - 6);
                }
                else
                {
                    return this.ConnectUserId;
                }
               
            }
        }

        public User user;
    }
}
