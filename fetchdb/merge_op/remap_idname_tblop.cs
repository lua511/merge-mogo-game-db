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
    // we just process a simple situation: one dbid & one name, also, the two are the same avatar
    class remap_idname_tblop
    {
        private static readonly string file_data_name = @"idname_map.dat";
        private static readonly string config_file_name = @"idname_table_config.xml";

        private Dictionary<string, KeyValuePair<string,string>> configed_tables;

        private cachedata profile;
        private data.database curr_conn;

        private Dictionary<string, KeyValuePair<UInt64, string>> fakekey2keyname;

        public remap_idname_tblop(data.database connection,cachedata cdata)
        {
            profile = cdata;
            curr_conn = connection;
        }

        public void process(workspace wspace)
        {
            load_config_file();
            load_fakekey_dbidname_cache(wspace);
            if(System.IO.File.Exists(file_data_name))
            {
                // load
                load_from_data_file(wspace);
            }
            else
            {
                create_alonewith_database_operation(wspace);
            }
            if(!post_check_data(wspace))
            {
                create_alonewith_database_operation(wspace);
            }
            if (!post_check_data(wspace))
            {
                throw new Exception("post check fail,remap id name tables");
            }
        }

        private void load_config_file()
        {
            configed_tables = new cqx_wraper.qxtable_loader().load_by_pair(config_file_name);
        }

        private void load_fakekey_dbidname_cache(workspace wspace)
        {
            fakekey2keyname = new Dictionary<string, KeyValuePair<ulong, string>>();
            foreach(var nava in profile.Remaped_Avatars)
            {
                var fakekey = safeformat.get_keystr_from_dbid_serverid(nava.old_dbid, nava.serverid);
                fakekey2keyname.Add(fakekey, new KeyValuePair<ulong, string>(nava.new_dbid, nava.new_name));
            }
        }

        private KeyValuePair<UInt64,string>  get_info_by_fakekey(UInt64 old_dbid,string serverid)
        {
            var fake_key = safeformat.get_keystr_from_dbid_serverid(old_dbid, serverid);
            return fakekey2keyname[fake_key];
        }

        void load_from_data_file(workspace wspace)
        {
            profile.Remaped_IdNameTables = new cpe_wraper.serialize_op<List<TableIdNameInfoSet>>().load(file_data_name);
        }

        void create_alonewith_database_operation(workspace wspace)
        {
            var all_data = new List<TableIdNameInfoSet>();
            foreach(var cfgtbl in configed_tables)
            {
                var table_name = new data.tbname(cfgtbl.Key);
                // drop , create
                new cmd_wraper.table_cmd(curr_conn, table_name, wspace.TargetDb).drop_table(true);
                new cnl_wrapper.nltable_cmd(curr_conn, wspace.TargetDb, table_name).copy_from(wspace.SchemaDatabase);
                // fill data
                foreach(var v in wspace.SourceDbs)
                {
                    new cnl_wrapper.nltable_cmd(curr_conn, wspace.TargetDb, table_name).fill_from(v);
                }
            }
            // load data
            foreach(var cfgtbl in configed_tables)
            {
                var table_set = new TableIdNameInfoSet();
                table_set.table_name = new data.tbname(cfgtbl.Key);
                table_set.Desc = new TableIdNameDesc();
                table_set.Desc.left_name = cfgtbl.Value.Key;
                table_set.Desc.right_name = cfgtbl.Value.Value;
                var tinr = new cog_wraper.table_idname_remap_reader(table_set.Desc);
                new cmd_wraper.table_cmd(curr_conn, new data.tbname(cfgtbl.Key), wspace.TargetDb).load_records(tinr);
                table_set.Records = tinr.Values;
                all_data.Add(table_set);
            }

            // remap id / name
            foreach(var tins in all_data)
            {
                var desc = tins.Desc;
                for(int i = 0;i < tins.Records.Count; ++i)
                {
                    var data = tins.Records[i];
                    var kvp = get_info_by_fakekey(data.record_id, data.serverid);
                    tins.Records[i].record_id = kvp.Key;
                    tins.Records[i].record_name = kvp.Value;
                }
            }
            // save all
            Action<TableIdNameDesc, List<TableIdNameInfo>, data.tbname> act = (x, y, z) =>
              {
                  var rinw = new cog_wraper.remap_idname_batch_writer(x, y);
                  new cmd_wraper.table_cmd(curr_conn, z, wspace.TargetDb).update_record(rinw);
              };
            var wait_for_saving = new List<TableIdNameInfo>();
            int page_size = 1000;
            foreach (var tins in all_data)
            {
                var desc = tins.Desc;
                foreach (var v in tins.Records)
                {
                    wait_for_saving.Add(v);
                    if (wait_for_saving.Count > page_size)
                    {
                        act(desc, wait_for_saving, tins.table_name);
                        wait_for_saving.Clear();
                    }
                }
                if (wait_for_saving.Count > 0)
                {
                    act(desc, wait_for_saving, tins.table_name);
                    wait_for_saving.Clear();
                }
            }
            // data to local disk
            profile.Remaped_IdNameTables = all_data;
            new cpe_wraper.serialize_op<List<TableIdNameInfoSet>>().save(file_data_name, profile.Remaped_IdNameTables);
        }

        bool post_check_data(workspace wspace)
        {
            if(profile.Remaped_IdNameTables.Count != configed_tables.Count)
            {
                return false;
            }
            foreach(var v in profile.Remaped_IdNameTables)
            {
                if(!configed_tables.ContainsKey(v.table_name.table_name))
                {
                    return false;
                }
            }
            foreach(var v in configed_tables)
            {
                bool bexist = false;
                foreach(var kvp in profile.Remaped_IdNameTables)
                {
                    if(kvp.table_name.table_name == v.Key)
                    {
                        bexist = true;
                        break;
                    }
                }
                if(!bexist)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */