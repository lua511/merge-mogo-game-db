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
    // 1. we have a merged dat file, load it
    // 2. there isn't a dat file, create target table & fill adaptor & create dat file
    // 3. check the dat file with db table (simply, just count)
    class avatar_tblop
    {
        private static readonly string file_data_name = @"merge_avatar.dat";
        private static readonly data.tbname avatar_table_name = global_config.avatar_table_name;
        private cachedata profile;
        private data.database current_connection;
        public avatar_tblop(data.database connection,cachedata cdata)
        {
            current_connection = connection;
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
                Console.WriteLine("avatar table use time : " + watch.ElapsedMilliseconds / 1000);
            }
            post_check_data(wspace);
        }

        public void load_from_data_file(workspace wspace)
        {
            profile.All_Avatars = new cpe_wraper.serialize_op<List<AvatarInfo>>().load(file_data_name);
        }

        public void create_alonewith_database_operation(workspace wspace)
        {
            new cmd_wraper.table_cmd(current_connection, avatar_table_name, wspace.TargetDb).drop_table(true);
            new cnl_wrapper.nltable_cmd(current_connection,wspace.TargetDb,avatar_table_name).copy_from(wspace.SchemaDatabase);
            // fill adaptor
            foreach(var db in wspace.SourceDbs)
            {
                new cnl_wrapper.nltable_cmd(current_connection,wspace.TargetDb,avatar_table_name).fill_from(db);
            }
            // load all to cache
            var avatar_reader = new cog_wraper.avatar_summary_reader();
            new cog_wraper.ogtable_reader<cog_wraper.avatar_summary_reader>(avatar_reader, current_connection, wspace.TargetDb,avatar_table_name).load_summary();
            var datas = avatar_reader.Values;
            profile.All_Avatars = datas;
            // save cache to disk
            new cpe_wraper.serialize_op<List<AvatarInfo>>().save(file_data_name, datas);
        }
        public void post_check_data(workspace wspace)
        {
            var merged_count = new cmd_wraper.table_cmd(current_connection,avatar_table_name,wspace.TargetDb).get_record_count();
            if(merged_count != profile.All_Avatars.Count)
            {
                throw new Exception("post_check failed,avatar table");
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
