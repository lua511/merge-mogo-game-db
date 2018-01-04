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
    class account_record_deletor : cpe_wraper.idelete_adaptor
    {
        public UInt64 dbid;
        public account_record_deletor(UInt64 dbid)
        {
            this.dbid = dbid;
        }

        public string sql_where
        {
            get
            {
                return @"id = @id";
            }
        }
        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue(@"@id", dbid);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
