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
using fetchdb.data;

namespace fetchdb.op_wraper
{
    class rename_avatar
    {
        private cachedata cached;

        private List<string> old_names;
        private List<string> conficted_names;

        private Dictionary<UInt64, string> renamed_names;

        public rename_avatar(cachedata cd)
        {
            cached = cd;
            old_names = new List<string>();
            conficted_names = new List<string>();
            renamed_names = new Dictionary<ulong, string>();
        }

        private List<AvatarInfo> GetAvatarsWithByOldName(string old_name)
        {
            var dst = new List<AvatarInfo>();
            foreach(var v in cached.All_Avatars)
            {
                if(v.old_name == old_name)
                {
                    dst.Add(v);
                }
            }
            return dst;
        }

        // how to rename
        //   1. rolename + server_id
        //   2. if exist, try rolename + auto_inc id
        public Dictionary<UInt64, string> process(workspace ws)
        {
            var all_avatar = cached.All_Avatars;

            foreach(var v in all_avatar)
            {
                if(!old_names.Contains(v.old_name))
                {
                    old_names.Add(v.old_name);
                }
                else
                {
                    conficted_names.Add(v.old_name);
                }
            }
            
            foreach(var conficted in conficted_names)
            {
                var avatars = GetAvatarsWithByOldName(conficted);
                foreach(var ava in avatars)
                {
                    var try_new_name = ava.old_name + ws.GetSourceDbIndex(ava.serverid).ToString();
                    if(!old_names.Contains(try_new_name))
                    {
                        ava.new_name = try_new_name;
                        old_names.Add(try_new_name);
                        renamed_names.Add(ava.new_dbid, ava.new_name);
                    }
                    if(string.IsNullOrEmpty(ava.new_name))
                    {
                        int cur_idx = ws.SourceDbs.Count + 1;
                        for(;cur_idx < 100; ++cur_idx)
                        {
                            try_new_name = ava.old_name + cur_idx.ToString();
                            if(!old_names.Contains(try_new_name))
                            {
                                ava.new_name = try_new_name;
                                old_names.Add(try_new_name);
                                renamed_names.Add(ava.new_dbid, ava.new_name);
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(ava.new_name))
                    {
                        throw new Exception("too many rename tries,give up");
                    }
                }
            }
            return renamed_names;
        }

        public void fill_all_newnames(workspace ws, Dictionary<UInt64,string> renameds)
        {
            foreach (var kvp in cached.All_Avatars)
            {
                if (renameds.ContainsKey(kvp.new_dbid))
                {
                    
                }
                else
                {
                    kvp.new_name = kvp.old_name;
                }
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
