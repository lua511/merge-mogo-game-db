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

namespace fetchdb.merge_op
{
    class rename_avatar
    {
        private static readonly string file_data_name = "remaped_avatar.dat";
        private static readonly data.tbname avatar_table_name = global_config.avatar_table_name;
        private data.database curr_conn;
        private cachedata cached;

        private HashSet<string> old_names;
        private HashSet<string> conficted_names;
        private Dictionary<string, List<AvatarInfo>> old_name_to_avatars;
        private Dictionary<UInt64, string> renamed_names;

        public rename_avatar(data.database conn,cachedata cd)
        {
            curr_conn = conn;
            cached = cd;
            old_names = new HashSet<string>();
            conficted_names = new HashSet<string>();
            old_name_to_avatars = new Dictionary<string, List<AvatarInfo>>();
            renamed_names = new Dictionary<ulong, string>();
        }

        public void process(workspace wspace)
        {
            if(System.IO.File.Exists(file_data_name))
            {
                // load
                load_from_data_file(wspace);
            }
            else
            {
                create_data_alonewith_save_db_file(wspace);
            }
            post_check_data(wspace);
        }

        public void load_from_data_file(workspace wspace)
        {
            cached.Remaped_Avatars = new cpe_wraper.serialize_op<List<AvatarInfo>>().load(file_data_name);
        }

        private List<AvatarInfo> GetAvatarsInfoByOldName(string old_name)
        {
            if (old_name_to_avatars.ContainsKey(old_name))
            {
                return old_name_to_avatars[old_name];
            }
            throw new Exception("not valid name");
        }

        // how to rename
        //   1. rolename + server_id
        //   2. if exist, try rolename + auto_inc id
        void create_data_alonewith_save_db_file(workspace ws)
        {
            // clear context
            old_names.Clear();
            conficted_names.Clear();
            renamed_names.Clear();
            old_name_to_avatars.Clear();

            var all_avatar = cached.All_Avatars;

            // cache names & conficted names
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
                if(!old_name_to_avatars.ContainsKey(v.old_name))
                {
                    old_name_to_avatars.Add(v.old_name, new List<AvatarInfo>());
                }
                old_name_to_avatars[v.old_name].Add(v);
            }
            // rename
            foreach(var conficted in conficted_names)
            {
                var avatars = GetAvatarsInfoByOldName(conficted);
                foreach(var ava in avatars)
                {
                    bool bRenamed = false;
                    var try_new_name = ava.old_name + ws.GetSourceDbIndex(ava.serverid).ToString();
                    if(!old_names.Contains(try_new_name))
                    {
                        old_names.Add(try_new_name);
                        renamed_names.Add(ava.new_dbid, try_new_name);
                        bRenamed = true;
                    }
                    if(!bRenamed)
                    {
                        int cur_idx = ws.SourceDbs.Count + 1;
                        for(;cur_idx < 100; ++cur_idx)
                        {
                            try_new_name = ava.old_name + cur_idx.ToString();
                            if(!old_names.Contains(try_new_name))
                            {
                                old_names.Add(try_new_name);
                                renamed_names.Add(ava.new_dbid, try_new_name);
                                bRenamed = true;
                                break;
                            }
                        }
                    }
                    if (!bRenamed)
                    {
                        throw new Exception("too many rename tries,give up");
                    }
                }
            }
            old_names.Clear();
            // apply new name
            var target_avatars = new List<AvatarInfo>();
            foreach (var kvp in cached.All_Avatars)
            {
                var p = kvp.deep_copy();

                if (renamed_names.ContainsKey(p.new_dbid))
                {
                    p.new_name = renamed_names[p.new_dbid];
                }
                else
                {
                    p.new_name = p.old_name;
                }
                target_avatars.Add(p);

                if(old_names.Contains(p.new_name))
                {
                    throw new Exception("rename failed");
                }
                else
                {
                    old_names.Add(p.new_name);
                }
            }
            cached.Remaped_Avatars = target_avatars;

            var watcher = new System.Diagnostics.Stopwatch();
            watcher.Start();
            // save to db
            int page_count = 1000;
            var avatars_for_save = new List<AvatarInfo>();
            Action<List<AvatarInfo>> action = (o) => {
                var anw = new cog_wraper.avatar_name_batch_writer(avatars_for_save);
                new cog_wraper.ogtable_writer<cog_wraper.avatar_name_batch_writer>(anw, curr_conn, ws.TargetDb, avatar_table_name).save_summary();
            };

            foreach (var kvp in target_avatars)
            {
                if(renamed_names.ContainsKey(kvp.new_dbid))
                {
                    avatars_for_save.Add(kvp);
                    if (avatars_for_save.Count >= page_count)
                    {
                        action(avatars_for_save);
                        avatars_for_save.Clear();
                    }
                }
            }
            if(avatars_for_save.Count > 0)
            {
                action(avatars_for_save);
                avatars_for_save.Clear();
            }
            watcher.Stop();
            Console.WriteLine("save new names : " + watcher.ElapsedMilliseconds / 1000);

            // save to disk
            new cpe_wraper.serialize_op<List<AvatarInfo>>().save(file_data_name, cached.Remaped_Avatars);
        }

        void post_check_data(workspace ws)
        {
            // just check count & 2 sample
            if(cached.Remaped_Avatars.Count != cached.All_Avatars.Count)
            {
                throw new Exception("post check failed,rename avatar table");
            }
            var renamed_cache = new List<AvatarInfo>();
            foreach(var kvp in cached.Remaped_Avatars)
            {
                if(kvp.new_name != kvp.old_name)
                {
                    renamed_cache.Add(kvp);
                }
            }
            // todo: random check
            for(int i = 0;i < 2 & i < renamed_cache.Count; ++i)
            {
                var ai = renamed_cache[i];
                var anr = new cog_wraper.avatar_name_reader(ai.new_dbid);
                new cog_wraper.ogtable_reader<cog_wraper.avatar_name_reader>(anr, curr_conn, ws.TargetDb, avatar_table_name).load_summary();
                if(anr.Value != ai.new_name)
                {
                    throw new Exception("post check failed,avatar new name");
                }
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
