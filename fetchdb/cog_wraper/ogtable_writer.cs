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

namespace fetchdb.cog_wraper
{
    class ogtable_writer<U> where U : cpe_wraper.iwrite_adaptor
    {
        private U adaptor;
        private data.dbname database_name;
        private data.tbname table_name;
        private data.database curr_conn;
        public ogtable_writer(U data, data.database conn,data.dbname database_name,data.tbname table_name)
        {
            this.adaptor = data;
            this.curr_conn = conn;
            this.table_name = table_name;
            this.database_name = database_name;
        }

        public void save_summary()
        {
            new cmd_wraper.table_cmd(curr_conn, table_name, database_name).update_record(adaptor);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */