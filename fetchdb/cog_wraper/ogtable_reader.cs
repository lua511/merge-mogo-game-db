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

// cog
//  1. alphabetic: cmd/cnl/cog
//  2. g means game,sometimes g is just like game adaptor
namespace fetchdb.cog_wraper
{
    class ogtable_reader<U> where U : cpe_wraper.iread_adaptor
    {
        private U adaptor;
        private data.dbname database_name;
        private data.tbname table_name;
        private data.database curr_conn;
        public ogtable_reader(U adaptor, data.database conn, data.dbname database_name, data.tbname table_name)
        {
            this.adaptor = adaptor;
            this.database_name = database_name;
            this.table_name = table_name;
            curr_conn = conn;
        }
        public void load_summary()
        {
            var sql_text = @"select {0} from {1}.{2}";
            var where_case = adaptor.sql_where;
            if(!string.IsNullOrEmpty(where_case))
            {
                sql_text = sql_text + @" where {3}";
                sql_text = safeformat.format(sql_text, adaptor.sql_select, database_name.database_name, table_name.table_name, where_case);
            }
            else
            {
                sql_text = safeformat.format(sql_text, adaptor.sql_select, database_name.database_name, table_name.table_name);
            }
            var tcmd = new cmd_wraper.table_cmd(curr_conn, table_name, database_name);
            tcmd.load_records(adaptor);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
