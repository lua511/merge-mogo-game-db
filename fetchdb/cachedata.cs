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
        private List<AvatarInfo> all_avatars_info = null;
        private List<AccountInfo> all_accounts_info = null;

        // carefully use it, todo: refactor, should have some access limit.
        public List<AvatarInfo> All_Avatars
        {
            get { return all_avatars_info; }
            set { all_avatars_info = value; }
        }
        public List<AccountInfo> All_Account
        {
            get { return all_accounts_info; }
            set { all_accounts_info = value; }
        }

        private List<AvatarInfo> remaped_avatars_info = null;
        private List<AccountInfo> remaped_accounts_info = null;

        public List<AvatarInfo> Remaped_Avatars
        {
            get { return remaped_avatars_info; }
            set { remaped_avatars_info = value; }
        }
        public List<AccountInfo> Remaped_Accounts
        {
            get { return remaped_accounts_info; }
            set { remaped_accounts_info = value; }
        }
        
        public List<string> Simple_Tables
        {
            get;set;
        }

        public List<string> One_Tables
        {
            get;set;
        }

        public List<string> DropAll_Tables
        { get; set; }

        public List<TableIdInfoSet>   Remaped_IdTables
        {
            get;set;
        }

        public void store_avatar(string name,UInt64 dbid,string server_db)
        {
            var ai = new AvatarInfo();
            ai.old_name = name;
            ai.old_dbid = dbid;
            ai.serverid = server_db;
            all_avatars_info.Add(ai);
        }

        public void avatar_newid(UInt64 new_dbid,UInt64 old_dbid,string server_db)
        {
            foreach(var v in all_avatars_info)
            {
                if(v.old_dbid == old_dbid && v.serverid == server_db)
                {
                    v.new_dbid = new_dbid;
                    break;
                }
            }
        }

        public AvatarInfo get_avatarinfo_by_oldinfo(UInt64 old_id,string old_server)
        {
            foreach(var v in all_avatars_info)
            {
                if(v.old_dbid == old_id && v.serverid == old_server)
                {
                    return v;
                }
            }
            return null;
        }

        public  bool is_avatar_newid_allloaded()
        {
            for(int i =0; i< 10 && i < all_avatars_info.Count; ++i)
            {
                var ai = all_avatars_info[i];
                if(ai.new_dbid < int.MaxValue)
                {
                    return false;
                }
            }
            return true;
        }
        public bool is_avatar_newname_allfilled()
        {
            for (int i = 0; i < 10 && i < all_avatars_info.Count; ++i)
            {
                var ai = all_avatars_info[i];
                if (string.IsNullOrEmpty(ai.new_name))
                {
                    return false;
                }
            }
            return true;
        }

        public void store_account_summary(AccountInfo ai,string server_db)
        {
            all_accounts_info.Add(new AccountInfo() { old_name = ai.old_name,old_dbid = ai.old_dbid,old_avatarinfo = ai.old_avatarinfo,serverid = server_db });
        }

        public void account_newid(UInt64 new_id,UInt64 old_id,string old_server)
        {
            foreach(var v in all_accounts_info)
            {
                if(v.old_dbid == old_id && v.serverid == old_server)
                {
                    v.new_dbid = new_id;
                    break;
                }
            }
        }

        public bool is_account_newid_allloaded()
        {
            foreach(var kvp in all_accounts_info)
            {
                if(kvp.new_dbid < int.MaxValue)
                {
                    return false;
                }
            }
            return true;
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
                all_accounts_info = cinfo.all_accounts_info;
                if(all_accounts_info == null)
                {
                    all_accounts_info = new List<AccountInfo>();
                }
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
