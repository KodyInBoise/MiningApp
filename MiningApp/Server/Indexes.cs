using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class Indexes
    {
        public class Clients
        {
            public static int GetIndex(string key) => IndexDictionary[key];

            public static int ClientID => IndexDictionary["ClientID"];
            public static int UserID => IndexDictionary["UserID"];
            public static int LastCheckin => IndexDictionary["LastCheckin"];

            public static Dictionary<string, int> IndexDictionary = new Dictionary<string, int>()
            {
                { "ClientID", 0 },
                { "UserID", 1 },
                { "LastCheckin", 2 },
            };
        }
    }
}
