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
    // methods:
    //   1. show databases
    //   2. run sql_text_cmd (if you have to use,here it is)
    //   3. format sql text
    class connection_cmd
    {
        private data.database current_db;
        public connection_cmd(data.database db)
        {
            current_db = db;
        }

        // 1. show databases
        public List<string> show_databases()
        {
            var act = new cpe_wraper.StringReaderAdaptor(0, "", "");
            var cmd = current_db.prepare_read_cmd(@"show databases");
            act.update(cmd);
            current_db.execute_read(cmd,act.load);
            return act.Values;
        }

        // 2. run_sql_text_cmd
        public void run_sql_text(string sql)
        {
            var cmd = current_db.prepare_write_cmd(sql);
            current_db.execute_nonquery(cmd);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
