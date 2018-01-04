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
    class remap_account
    {
        private static readonly string file_data_name = @"remaped_account.dat";
        private static readonly data.tbname account_table_name = global_config.account_table_name;

        private cachedata cached;
        private data.database curr_conn;

        public remap_account(data.database conn, cachedata cd)
        {
            curr_conn = conn;
            cached = cd;
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
            cached.Remaped_Accounts = new cpe_wraper.serialize_op<List<AccountInfo>>().load(file_data_name);
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
        public void create_data_alonewith_save_db_file(workspace ws)
        {
            // load avatar info cache
            var dbids_to_new = new Dictionary<string, UInt64>();
            var dbids_to_name = new Dictionary<string, string>();
            var dbid_to_avatar = new Dictionary<UInt64, AvatarInfo>();
            foreach(var ava_info in cached.Remaped_Avatars)
            {
                var fake_key = safeformat.get_keystr_from_dbid_serverid(ava_info.old_dbid, ava_info.serverid);
                dbids_to_new.Add(fake_key, ava_info.new_dbid);
                dbids_to_name.Add(fake_key, ava_info.new_name);
                dbid_to_avatar.Add(ava_info.new_dbid, ava_info);
            }

            // load all accounts
            var avatarsInfo_all = new Dictionary<string, List<string>>();
            avatarsInfo_all.Clear();
            foreach(var acci in cached.All_Account)
            {
                if(!avatarsInfo_all.ContainsKey(acci.old_name))
                {
                    avatarsInfo_all.Add(acci.old_name, new List<string>());
                }
                avatarsInfo_all[acci.old_name].Add(acci.old_avatarinfo);
            }

            // first round,changed dbid & name
            var accounts = new List<AccountInfo>();
            foreach(var acc in cached.All_Account)
            {
                var p = acc.deep_copy();
                var avatarsInfo = new scriptdata.AvatarsInfo();
                avatarsInfo.LoadFromString(p.old_avatarinfo);
                // change id & name
                foreach(var ava in avatarsInfo.AllAvatars)
                {
                    var old_id = ava.id;
                    var server_id = p.serverid;
                    var fake_key = safeformat.get_keystr_from_dbid_serverid(old_id, server_id);
                    var new_id = dbids_to_new[fake_key];
                    var new_name = dbids_to_name[fake_key];
                    ava.id = new_id;
                    ava.name = new_name;
                }
                p.new_avatarinfo = avatarsInfo.SaveToString();
                accounts.Add(p);
            }

            var keeps_acc_ids = new HashSet<UInt64>();
            var merged_acc_ids = new HashSet<UInt64>();
            var new_accounts = new Dictionary<string,AccountInfo>();
            foreach(var acc in accounts)
            {
                if(new_accounts.ContainsKey(acc.old_name))
                {
                    // merge
                    var old_acc = new_accounts[acc.old_name];
                    var cur_acc = acc;

                    var c1 = new scriptdata.AvatarsInfo();
                    c1.LoadFromString(old_acc.new_avatarinfo);
                    var c2 = new scriptdata.AvatarsInfo();
                    c2.LoadFromString(cur_acc.new_avatarinfo);
                    c1.Merge(c2);
                    old_acc.new_avatarinfo = c1.SaveToString();

                    merged_acc_ids.Add(acc.new_dbid);
                }
                else
                {
                    // simple add
                    new_accounts.Add(acc.old_name, acc);

                    keeps_acc_ids.Add(acc.new_dbid);
                }
            }

            accounts.Clear();
            foreach(var kvp in new_accounts)
            {
                accounts.Add(kvp.Value);
            }

            cached.Remaped_Accounts = accounts;

            var watcher = new System.Diagnostics.Stopwatch();
            watcher.Start();
            // save to db 
            foreach (var dbid in merged_acc_ids)
            {
                var ard = new cog_wraper.account_record_deletor(dbid);
                new cog_wraper.ogtable_deletor<cog_wraper.account_record_deletor>(ard, curr_conn, ws.TargetDb, account_table_name).delete_summary();
            }
            var count = cached.Remaped_Accounts.Count;
            int page_count = 1000;
            for(var i = 0;i < count; ++i)
            {
                var little_set = new List<AccountInfo>();
                var start = i * page_count;
                int j = start;
                for(; j < start + page_count && j < count;++j)
                {
                    little_set.Add(cached.Remaped_Accounts[j]);
                }
                if(little_set.Count == 0)
                {
                    break;
                }
                var aaw = new cog_wraper.account_avatarsinfo_batch_writer(little_set);
                new cog_wraper.ogtable_writer<cog_wraper.account_avatarsinfo_batch_writer>(aaw, curr_conn, ws.TargetDb, account_table_name).save_summary();
            }
            watcher.Stop();
            Console.WriteLine("save new accounts : " + watcher.ElapsedMilliseconds / 1000);

            // save to disk
            new cpe_wraper.serialize_op<List<AccountInfo>>().save(file_data_name, cached.Remaped_Accounts);
        }

        public void post_check_data(workspace wspace)
        {
            var merged_count = new cmd_wraper.table_cmd(curr_conn, account_table_name, wspace.TargetDb).get_record_count();
            if (merged_count != cached.Remaped_Accounts.Count)
            {
                throw new Exception("post_check failed,remap account table");
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
