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
    // todo : refactor, from config file will be better
    class simplecopy_tblop
    {
        private static readonly string file_data_name = @"simple_cpy.dat";
        private static readonly string config_file_name = @"simple_table_config.xml";
        private List<string> configed_tables;

        private data.database curr_conn;
        private cachedata profile;
        public simplecopy_tblop(data.database conn,cachedata cache)
        {
            this.curr_conn = conn;
            this.profile = cache;
        }

        public void process(workspace wspace)
        {
            load_config_file();
            if(System.IO.File.Exists(file_data_name))
            {
                // load
                load_from_data_file(wspace);
            }
            else
            {
                // create
                create_alonewith_database_operation(wspace);
            }
            if(!post_check_data(wspace))
            {
                create_alonewith_database_operation(wspace);
            }
            if(!post_check_data(wspace))
            {
                throw new Exception("post check fail,simple tables");
            }
        }

        void load_config_file()
        {
            configed_tables = new cqx_wraper.qxtable_loader().load_by_table(config_file_name);
        }

        void load_from_data_file(workspace wspace)
        {
            profile.Simple_Tables = new cpe_wraper.serialize_op<List<string>>().load(file_data_name);
        }

        void create_alonewith_database_operation(workspace wspace)
        {
            var dst_tables = new List<string>();
            foreach(var v in configed_tables)
            {
                var tbl_name = new data.tbname(v);
                new cmd_wraper.table_cmd(curr_conn, tbl_name, wspace.TargetDb).drop_table(true);
                new cnl_wrapper.nltable_cmd(curr_conn, wspace.TargetDb, tbl_name).clone_from(wspace.SchemaDatabase);
                dst_tables.Add(v);
            }
            profile.Simple_Tables = dst_tables;
            // save to disk
            new cpe_wraper.serialize_op<List<string>>().save(file_data_name, profile.Simple_Tables);
        }

        bool post_check_data(workspace wspace)
        {
            foreach(var v in configed_tables)
            {
                if(!profile.Simple_Tables.Contains(v))
                {
                    return false;
                }
            }
            foreach (var v in profile.Simple_Tables)
            {
                if (!configed_tables.Contains(v))
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