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
    class remapid_tblop
    {
        private static readonly string file_data_name = @"idmap.dat";
        private static readonly string config_file_name = @"idreamap_table_config.xml";

        private Dictionary<string, List<string>> configed_tables;

        private cachedata profile;
        private data.database curr_conn;

        private Dictionary<string, UInt64> fakekey2dbid;

        public remapid_tblop(data.database connection,cachedata cdata)
        {
            profile = cdata;
            curr_conn = connection;
        }
        public void process(workspace wspace)
        {
            load_config_file();
            load_fakekey_dbid_cache(wspace);
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
            if(!post_check_data(wspace))
            {
                throw new Exception("post check fail,remapid tables");
            }
        }

        private void load_config_file()
        {
            configed_tables = new cqx_wraper.qxtable_loader().load_by_element(config_file_name);
        }

        private void load_fakekey_dbid_cache(workspace wspace)
        {
            fakekey2dbid = new Dictionary<string, ulong>();
            foreach(var nava in profile.Remaped_Avatars)
            {
                var fakekey = safeformat.get_keystr_from_dbid_serverid(nava.old_dbid, nava.serverid);
                fakekey2dbid.Add(fakekey, nava.old_dbid);
            }
        }

        private UInt64 get_dbid_by_fakekey(UInt64 old_dbid,string serverid)
        {
            var fake_key = safeformat.get_keystr_from_dbid_serverid(old_dbid, serverid);
            return fakekey2dbid[fake_key];
        }

        private void load_from_data_file(workspace wspace)
        {
            profile.Remaped_IdTables = new cpe_wraper.serialize_op<List<TableIdInfoSet>>().load(file_data_name);
        }

        private void create_alonewith_database_operation(workspace wspace)
        {
            var all_data = new List<TableIdInfoSet>();
            foreach (var cfgtbl in configed_tables)
            {
                var table_name = new data.tbname(cfgtbl.Key);
                // create table
                new cmd_wraper.table_cmd(curr_conn, table_name, wspace.TargetDb).drop_table(true);
                new cnl_wrapper.nltable_cmd(curr_conn, wspace.TargetDb, table_name).copy_from(wspace.SchemaDatabase);
                // fill data
                foreach(var v in wspace.SourceDbs)
                {
                    new cnl_wrapper.nltable_cmd(curr_conn, wspace.TargetDb, table_name).fill_from(v);
                }
            }

            // local datase
            foreach(var cfgtbl in configed_tables)
            {
                var table_set = new TableIdInfoSet();
                table_set.table_name = new data.tbname(cfgtbl.Key);
                table_set.Desc = new TableIdInfoDesc();
                table_set.Desc.columns = new List<string>();
                foreach (var key_name in cfgtbl.Value)
                {
                    table_set.Desc.columns.Add(key_name);
                }
                var tir = new cog_wraper.table_idremap_reader(table_set.Desc);
                new cmd_wraper.table_cmd(curr_conn, new data.tbname(cfgtbl.Key), wspace.TargetDb).load_records(tir);
                table_set.Records = tir.Values;
                all_data.Add(table_set);
            }

            // remap ids
            foreach(var tis in all_data)
            {
                var desc = tis.Desc;

                foreach(var record in tis.Records)
                {
                    for(int i = 0;i < record.values.Count; ++i)
                    {
                        var curr_v = record.values[i];
                        var server_id = record.serverid;
                        if (curr_v > 0)
                        {
                            var new_id = get_dbid_by_fakekey(curr_v, server_id);
                            record.values[i] = new_id;
                        }
                        else
                        {
                            record.values[i] = 0;
                        }
                    }
                }
            }

            Action<TableIdInfoDesc, List<TableIdInfo>,data.tbname> act = (x, y,z) =>
             {
                 var rbw = new cog_wraper.remapid_batch_writer(x, y);
                 new cmd_wraper.table_cmd(curr_conn, z, wspace.TargetDb).update_record(rbw);

             };
            List<TableIdInfo> wait_for_saving = new List<TableIdInfo>();
            int page_size = 1000; // change it if sql text is too long
            // save all ids
            foreach(var tis in all_data)
            {
                var desc = tis.Desc;
                foreach(var ti in tis.Records)
                {
                    // todo batch update
                    wait_for_saving.Add(ti);
                    if(wait_for_saving.Count > page_size)
                    {
                        // save batch
                        act(desc,wait_for_saving,tis.table_name);
                        wait_for_saving.Clear();
                    }
                }

                if (wait_for_saving.Count > 0)
                {
                    // save batch
                    act(desc,wait_for_saving,tis.table_name);
                }
            }
            // data to loacal disk
            profile.Remaped_IdTables = all_data;
            new cpe_wraper.serialize_op<List<TableIdInfoSet>>().save(file_data_name,profile.Remaped_IdTables);
        }

        private bool post_check_data(workspace wspace)
        {
            if (profile.Remaped_IdTables.Count != configed_tables.Values.Count)
            {
                return false;
            }
            bool all_exist = true;
            foreach(var v in profile.Remaped_IdTables)
            {
                if(!configed_tables.Keys.Contains(v.table_name.table_name))
                {
                    all_exist = false;
                    break;
                }
            }
            if(!all_exist)
            {
                return false;
            }
            foreach (var v in profile.Remaped_IdTables)
            {
                var tbl_count = new cmd_wraper.table_cmd(curr_conn, v.table_name, wspace.TargetDb).get_record_count();
                if(tbl_count != v.Records.Count)
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
