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

namespace fetchdb.cmd_wraper
{
    // want to change database? use database_cmd not database
    class database_cmd
    {
        private database current_db;
        public database_cmd(database db)
        {
            current_db = db;
        }

        public List<string> show_databases()
        {
            var act = new StringReaderAdaptor();
            current_db.execute_read(@"show databases", act.Load);
            return act.Values;
        }

        public void drop_database(string database_name)
        {
            var current_databases = show_databases();
            if (current_databases.Contains(database_name))
            {
                current_db.execute_nonquery(@"drop database " + database_name);
            }
        }

        public void create_database(string database_name)
        {

            var databases = show_databases();
            if (!databases.Contains(database_name))
            {
                current_db.execute_nonquery(@"create database " + database_name);
            }
        }

        public void execute_sql(string sql)
        {
            current_db.execute_nonquery(sql);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
