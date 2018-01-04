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
    class ogtable_deletor<U> where U : cpe_wraper.idelete_adaptor
    {
        private U adaptor;
        private data.dbname database_name;
        private data.tbname table_name;
        private data.database cur_conn;
        public ogtable_deletor(U adaptor,data.database conn,data.dbname database_name,data.tbname table_name)
        {
            this.adaptor = adaptor;
            this.database_name = database_name;
            this.table_name = table_name;
            this.cur_conn = conn;
        }

        public void delete_summary()
        {
            new cmd_wraper.table_cmd(cur_conn, table_name, database_name).delete_record(adaptor);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */