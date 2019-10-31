using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGameServer
{
    public class Class_Room
    {
        public List<Class_User> mUsers = new List<Class_User>();

        public int mId = 0;
        public string mMasterId = "";

        public string mName = "";

        public Class_Room()
        {

        }
    }
}
