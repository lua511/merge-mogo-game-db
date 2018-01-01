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
using System.Text;
using System.Threading.Tasks;

namespace fetchdb.op_wraper
{
    class remap_account
    {
        private cachedata cached;

        public remap_account(cachedata cd)
        {
            cached = cd;
        }

        private List<AccountInfo> get_account_by_name(string acc_name)
        {
            var dst = new List<AccountInfo>();
            foreach(var k in cached.All_Account)
            {
                if(k.old_name == acc_name)
                {
                    dst.Add(k);
                }
            }
            return dst;
        }

        // how to remap
        //  1. avatar dbid
        //  2. avatar name
        //  3. account merge

        public Dictionary<string,AccountInfo> process(workspace ws)
        {
            var all_account = cached.All_Account;

            var new_accounts = new Dictionary<string,AccountInfo>();

            // modify dbid/avatar_name
            foreach(var acc in all_account)
            {
                var parser = new scriptdata.AvatarsInfo();
                parser.LoadFromString(acc.old_avatarinfo);
                foreach(var av in parser.AllAvatars)
                {
                    var old_id = av.id;
                    var old_server = acc.serverid;
                    var rec = cached.get_avatarinfo_by_oldinfo(old_id, old_server);
                    if(rec == null)
                    {
                        av.is_empty = true;
                        continue;
                    }
                    var new_id = rec.new_dbid;
                    var new_name = rec.new_name;

                    av.name = new_name;
                    av.id = new_id;
                }
                acc.new_avatarinfo = parser.SaveToString();
            }
            // merge account
            foreach(var acc in all_account)
            {
                if(new_accounts.ContainsKey(acc.old_name))
                {
                    continue;
                }
                else
                {
                    var account_same_name = get_account_by_name(acc.old_name);
                    new_accounts.Add(acc.old_name, account_same_name[0]);

                    var parser = new scriptdata.AvatarsInfo();
                    parser.LoadFromString(account_same_name[0].new_avatarinfo);
                    for(int i = 1;i < account_same_name.Count; ++i)
                    {
                        var nacc = account_same_name[i];
                        var nparser = new scriptdata.AvatarsInfo();
                        nparser.LoadFromString(nacc.new_avatarinfo);
                        parser.Merge(nparser);
                    }

                    account_same_name[0].new_avatarinfo = parser.SaveToString();
                }
            }
            return new_accounts;
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
