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
    class remap_script_id_tblop
    {
        private static readonly string file_data_name = @"script_ids.dat";
        private static readonly string config_file_name = @"script_idremap_config.xml";

        private List<KeyValuePair<string, string>> configed_columns;

        private cachedata profile;
        private data.database curr_conn;

        private Dictionary<string, UInt64> fakekey2dbid;

        public remap_script_id_tblop(data.database connection,cachedata cdata)
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
                throw new Exception("post check fail,remap script id tables");
            }
        }

        private void load_config_file()
        {
            configed_columns = new cqx_wraper.qxtable_loader().load_by_script_id(config_file_name);
        }

        void load_fakekey_dbid_cache(workspace wspace)
        {
            fakekey2dbid = new Dictionary<string, ulong>();
            foreach(var nava in profile.Remaped_Avatars)
            {
                var fakekey = safeformat.get_keystr_from_dbid_serverid(nava.old_dbid, nava.serverid);
                fakekey2dbid.Add(fakekey, nava.new_dbid);
            }
        }

        UInt64 get_dbid_by_fakekey(UInt64 old_dbid,string serverid)
        {
            if(old_dbid == 0)
            {
                return 0;
            }
            var fake_key = safeformat.get_keystr_from_dbid_serverid(old_dbid, serverid);
            return fakekey2dbid[fake_key];
        }

        void load_from_data_file(workspace wspace)
        {
            profile.RemapedScriptsTable = new cpe_wraper.serialize_op<List<ScriptIdInfoSet>>().load(file_data_name);
        }

        void create_alonewith_database_operation(workspace wspace)
        {
            var all_data = new List<ScriptIdInfoSet>();
            // load all
            foreach(var cfgtbl in configed_columns)
            {
                var tbl_name = new data.tbname(cfgtbl.Key);
                var column_name = cfgtbl.Value;
                var siis = new ScriptIdInfoSet();
                siis.table_name = tbl_name;
                siis.Desc = new ScriptIdInfoDesc();
                siis.Desc.column_name = cfgtbl.Value;
                // load data
                var sir = new cog_wraper.script_id_reader(siis.Desc);
                new cmd_wraper.table_cmd(curr_conn, tbl_name, wspace.TargetDb).load_records(sir);
                siis.Records = sir.Values;
                all_data.Add(siis);
            }
            // remap all(why not in same line? face to human is better than face to cpu)
            foreach(var s in all_data)
            {
                var desc = s.Desc;
                var table_name = s.table_name.table_name;
                var column_name = desc.column_name;
                var patterns = new cqx_wraper.qxtable_loader().load_pattern_by_script_id(config_file_name, table_name, column_name);
                foreach(var p in s.Records)
                {
                    p.new_info = p.old_info;
                    foreach(var pattern in patterns)
                    {
                        var oldinfo = p.new_info;
                        var p1 = pattern.Key;
                        var p2 = pattern.Value;
                        var matches = System.Text.RegularExpressions.Regex.Matches(p.new_info, p1);

                        foreach (System.Text.RegularExpressions.Match m in matches)
                        {
                            var secondinfo = m.Value;
                            var m2 = System.Text.RegularExpressions.Regex.Matches(m.Value, p2);
                            if(m2.Count > 1)
                            {
                                throw new Exception("too complex match,try a special method");
                            }
                            var m3 = UInt64.Parse(((System.Text.RegularExpressions.Match)m2[0]).Value);
                            var serverinfo = p.serverid;
                            var m4 = get_dbid_by_fakekey(m3, serverinfo);

                            var  replaced_info = secondinfo.Replace(m3.ToString(), m4.ToString());
                            oldinfo = oldinfo.Replace(secondinfo, replaced_info);
                        }
                        p.new_info = oldinfo;
                    }
                }                

            }
            Action<ScriptIdInfoDesc,List<ScriptIdInfo>,data.tbname> act = (x, y, z) => {
                var siw = new cog_wraper.script_id_writer(x, y);
                new cmd_wraper.table_cmd(curr_conn, z, wspace.TargetDb).update_record(siw);
            };
            // save to db all
            int page_size = 1000;
            foreach(var s in all_data)
            {
                var wait_for_saving = new List<ScriptIdInfo>();
                foreach(var p in s.Records)
                {
                    wait_for_saving.Add(p);
                    if(wait_for_saving.Count > page_size)
                    {
                        act(s.Desc, wait_for_saving, s.table_name);
                        wait_for_saving.Clear();
                    }
                }
                if(wait_for_saving.Count > 0)
                {
                    act(s.Desc, wait_for_saving, s.table_name);
                    wait_for_saving.Clear();
                }
            }

            // save to local disk
            profile.RemapedScriptsTable = all_data;
            new cpe_wraper.serialize_op<List<ScriptIdInfoSet>>().save(file_data_name, profile.RemapedScriptsTable);
        }

        bool post_check_data(workspace wspace)
        {
            if(configed_columns.Count == profile.RemapedScriptsTable.Count)
            {
                return true;
            }
            return false;
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */