using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient
{
    public class UserInfo
    {
        public int id;
        public string Name;
        public string Email;
        public int Rating;

        public UserInfo(NameValueCollection list)
        {
            id = int.Parse(list["id"]);
            Name = list["Name"];
            Email = list["Email"];
            Rating = int.Parse(list["Rating"]);
        }
    }
}
