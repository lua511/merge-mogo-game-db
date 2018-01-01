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
    class avatar_name_reader : cpe_wraper.iread_adaptor
    {
        private string dst;
        private UInt64 avatar_dbid;
        public avatar_name_reader(UInt64 avatar_dbid)
        {
            dst = string.Empty;
            this.avatar_dbid = avatar_dbid;
        }

        public string   Value
        {
            get
            {
                return dst;
            }
        }

        public string sql_select
        {
            get
            {
                return "sm_name";
            }
        }
        public string sql_where
        {
            get
            {
                return @"id=@id";
            }
        }

        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue(@"@id", avatar_dbid);
        }

        public void load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            if(rds.Read())
            {
                var name = rds.GetString(0);
                dst = name;
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */