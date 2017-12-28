/*
   +----------------------------------------------------------------------+
   | https://github.com/lua511/merge-mogo-game-db                         |
   +----------------------------------------------------------------------+
   | Author: winter yang <blueliuan@163.com>                              |
   +----------------------------------------------------------------------+  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace fetchdb
{
    [Serializable]
    class cachedata
    {
        public static cachedata Instance
        {
            get;set;
        }

        [Serializable]
        public class AvatarInfo
        {
            public string old_name { get; set; }
            public string new_name { get; set; }
            public UInt64 old_dbid { get; set; }
            public UInt64 new_dbid { get; set; }
            public string serverid { get; set; }
        }
        [Serializable]
        public class AccountInfo
        {
            public string old_name { get; set; }
            public UInt64 old_dbid { get; set; }
            public string serverid { get; set; }
        }
        private List<AvatarInfo> all_avatars_info = new List<AvatarInfo>();
        private List<AccountInfo> all_account_info = new List<AccountInfo>();

        public void store_avatar(string name,UInt64 dbid,string server_db)
        {
            var ai = new AvatarInfo();
            ai.old_name = name;
            ai.old_dbid = dbid;
            ai.serverid = server_db;
            all_avatars_info.Add(ai);
        }
        public void store_account(string name,UInt64 dbid,string server_db)
        {
            all_account_info.Add(new AccountInfo() { old_name = name,old_dbid = dbid,serverid = server_db });
        }


        public void Save(string file_name)
        {
            using (var fs = new System.IO.FileStream(file_name, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, this);
            }
        }
        public void Load(string file_name)
        {
            using (var fs = new System.IO.FileStream(file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                var bf = new BinaryFormatter();
                var cinfo = bf.Deserialize(fs) as cachedata;
                all_avatars_info = cinfo.all_avatars_info;
                if(all_avatars_info == null)
                {
                    all_avatars_info = new List<AvatarInfo>();
                }
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
