/*
   +----------------------------------------------------------------------+
   | https://github.com/lua511/merge-mogo-game-db                         |
   +----------------------------------------------------------------------+
   | Author: winter yang <blueliuan@163.com>                              |
   +----------------------------------------------------------------------+  
*/

using fetchdb.cmd_wraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fetchdb
{
    class merge
    {
        private workspace wspace;
        private static string avatars_file_name = "avatar.dat";

        // skip table that not generated from entity
        private bool is_table_need_skip(string table_name)
        {
            if(table_name.StartsWith("tbl_"))
            {
                return false;
            }
            return true;
        }

        public void main(workspace wspace)
        {           
            this.wspace = wspace;
            var cd = new cachedata();
            cachedata.Instance = cd;
            string opt_str;
            if (System.IO.File.Exists(avatars_file_name))
            {
                Console.WriteLine("found cache data,drop it ? (only full lower case 'yes' means yes/y)[no]");
                opt_str = Console.ReadLine();
                if(opt_str.Trim() == @"yes")
                {
                    System.IO.File.Delete(avatars_file_name);
                }
            }

            if (System.IO.File.Exists(avatars_file_name))
            {
                Console.WriteLine("start to load avatars from local disk");
                cd.Load(avatars_file_name);
            }
            else
            {
                Console.WriteLine("start to load avatars from mysql database");
                foreach(var db in wspace.SourceDbs)
                {
                    var datas = new table_cmd(database.Instance).load_key_name(db,"tbl_Avatar");
                    foreach(var kvp in datas)
                    {
                        cd.store_avatar(kvp.Value, kvp.Key, db);
                    }
                }
                Console.WriteLine("start to save cache to local disk");
                cd.Save(avatars_file_name);

                Console.WriteLine("start to load account from mysql database");
                foreach(var db in wspace.SourceDbs)
                {
                    var datas = new table_cmd(database.Instance).load_key_name(db, "tbl_Account");
                    foreach(var kvp in datas)
                    {
                        cd.store_account(kvp.Value, kvp.Key, db);
                    }
                }
                Console.WriteLine("start to save cache to local disk");
                cd.Save(avatars_file_name);
            }
            // drop exists
            Console.WriteLine("drop exist target database ?(only full lower case 'yes' means yes/y)[no]");
            opt_str = Console.ReadLine();
            if(opt_str.Trim() == "yes")
            {
                drop_exist_database();
            }
            // create target
            new database_cmd(database.Instance).create_database(wspace.TargetDb);

            // drop tables
            Console.WriteLine("drop exist tables ?(only full lower case 'yes' means yes/y[no]");
            opt_str = Console.ReadLine();
            if (opt_str.Trim() == "yes")
            {
                drop_exist_tables();
            }
            // create tables
            create_avatar_from_schema();
        }

        private void drop_exist_database()
        {
            new database_cmd(database.Instance).drop_database(wspace.TargetDb);
        }

        private void drop_exist_tables()
        {
            var dcmd = new table_cmd(database.Instance);
            
            var tables = dcmd.show_tables(wspace.TargetDb);
            foreach(var t in tables)
            {
                dcmd.drop_table(wspace.TargetDb, t);
            }
        }

        private void create_avatar_from_schema()
        {
            var tblc = new table_cmd(database.Instance);
            var table_name = "tbl_Avatar";
            var all_tables = tblc.show_tables(wspace.TargetDb);
            if(all_tables.Contains(table_name))
            {
                Console.WriteLine("avatar table exist,keep it?(only full lower case 'yes' means yes/y[no]");
                var opt = Console.ReadLine();
                if(opt.Trim() == "yes")
                {
                    // keep it
                }
                else
                {
                    tblc.drop_table(wspace.TargetDb,table_name);
                }
            }

            var schema = tblc.show_createtable(wspace.SchemaDatabase, table_name);
            all_tables = tblc.show_tables(wspace.TargetDb);
            if(!all_tables.Contains(table_name))
            {
                Console.WriteLine("start to create avatar table");
                tblc.copy_create_table_avatarlike(schema, wspace.TargetDb);
            }
            Console.WriteLine("init avatar database ? (only full lower case 'yes' means yes/y[no]");
            var opt_str = Console.ReadLine();
            if(opt_str.Trim() != "yes")
            {
                return;
            }
            Console.WriteLine("start to init avatar database");

            // load all data
            var columns_match = System.Text.RegularExpressions.Regex.Matches(schema, @"\n\s+\`[a-zA-Z_]+\`\s");
            List<string> usefull_columns = new List<string>();
            foreach(System.Text.RegularExpressions.Match m in columns_match)
            {
                var v = m.Value.Split('`');
                var name = v[1];
                if(name == "id")
                {
                    continue;
                }
                usefull_columns.Add(name);
            }

            var init_sql_schema = @"insert into {0}.{1} (old_id,old_server,{2}) select id as old_id,'{3}'as old_server, {2} from {3}.{1};";
            var column_strs = new StringBuilder();
            foreach(var v in usefull_columns)
            {
                column_strs.Append(v);
                column_strs.Append(",");
            }
            column_strs.Remove(column_strs.Length - 1, 1);
            foreach(var db in wspace.SourceDbs)
            {
                var init_sql = string.Format(init_sql_schema, wspace.TargetDb, table_name, column_strs.ToString(), db);
                new database_cmd(database.Instance).execute_sql(init_sql);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
