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
    class avatar_cmd
    {
        private database current_db;
        public avatar_cmd(database db)
        {
            current_db = db;
        }
       
        public void rename_avatar(UInt64 dbid,string new_name,string database_name)
        {
            var sql = @"update {0}.tbl_Avatar set sm_name = @sm_name where id = @id";
            var sql_txt = string.Format(sql, database_name);
            using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql_txt, current_db.Conn))
            {
                cmd.Parameters.AddWithValue(@"@id", dbid);
                cmd.Parameters.AddWithValue(@"@sm_name", new_name);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
