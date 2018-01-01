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
    class account_tblop
    {
        private static readonly string file_data_name = @"merge_account.dat";
        private static readonly data.tbname account_table_name = global_config.account_table_name;
        private cachedata profile;
        private data.database conn;
        public account_tblop(data.database connection,cachedata cdata)
        {
            conn = connection;
            profile = cdata;
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
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                // create
                create_alonewith_database_operation(wspace);
                watch.Stop();
                Console.WriteLine("account table use time : " + watch.ElapsedMilliseconds / 1000);
            }

            post_check_data(wspace);
        }

        public void load_from_data_file(workspace wspace)
        {
            profile.All_Account = new cpe_wraper.serialize_op<List<AccountInfo>>().load(file_data_name);
        }

        public void create_alonewith_database_operation(workspace wspace)
        {
            new cmd_wraper.table_cmd(conn, account_table_name, wspace.TargetDb).drop_table(true);
            new cnl_wrapper.nltable_cmd(conn,wspace.TargetDb, account_table_name).copy_from(wspace.SchemaDatabase);
            // fill adaptor
            foreach(var db in wspace.SourceDbs)
            {
                new cnl_wrapper.nltable_cmd(conn,wspace.TargetDb,account_table_name).fill_from(db);
            }
            // load all to cache
            var account_reader = new cog_wraper.account_summary_reader();
            new cog_wraper.ogtable_reader<cog_wraper.account_summary_reader>(account_reader, conn, wspace.TargetDb,account_table_name).load_summary();
            var datas = account_reader.Values;
            profile.All_Account = datas;
            // save cache to disk
            new cpe_wraper.serialize_op<List<AccountInfo>>().save(file_data_name, datas);
        }

        public void post_check_data(workspace wspace)
        {
            var merged_count = new cmd_wraper.table_cmd(conn, account_table_name, wspace.TargetDb).get_record_count();
            if(merged_count != profile.All_Account.Count)
            {
                throw new Exception("post_check failed,account table");
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */