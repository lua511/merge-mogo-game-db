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
    // methods:
    // database operations:
    //  1. exists
    //  2. drop database & safe drop database
    //  3. create database & force create new database
    // table operations:
    //  4. show tables

    class database_cmd
    {
        private data.database current_conn;
        private data.dbname db_name;
        public database_cmd(data.database db,data.dbname db_name)
        {
            current_conn = db;
            this.db_name = db_name;
        }
        // 1. exists
        public bool exists()
        {
            var dbs = new connection_cmd(current_conn).show_databases();
            return dbs.Contains(db_name.database_name);
        }
        // 2. drop database & safe drop databases
        public void drop_database(bool bSafe = true)
        {
            if(bSafe && !exists())
            {
                return;
            }
            var sql = safeformat.format(@"drop database {0}", db_name.database_name);
            new connection_cmd(current_conn).run_sql_text(sql);
        }
        // 3. create database
        public void create_database(bool bForce)
        {
            if(exists())
            {
                if(bForce)
                {
                    drop_database(false);
                }
                else
                {
                    return;
                }
            }
            var sql = safeformat.format(@"create database {0}", db_name.database_name);
            new connection_cmd(current_conn).run_sql_text(sql);
        }

        // 4. show tables
        public List<string> show_tables()
        {
            var ocs = new cpe_wraper.StringReaderAdaptor(0,"","");
            var txt = @"use {0};show tables";
            txt = safeformat.format(txt, db_name.database_name);
            var cmd = current_conn.prepare_read_cmd(txt);
            current_conn.execute_read(cmd, ocs.load);
            return ocs.Values;
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
