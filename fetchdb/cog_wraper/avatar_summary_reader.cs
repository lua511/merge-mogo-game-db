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
    class avatar_summary_reader : cpe_wraper.iread_adaptor
    {
        private List<AvatarInfo> dst = new List<AvatarInfo>();
        public avatar_summary_reader()
        {
            dst = new List<AvatarInfo>();
        }

        public List<AvatarInfo> Values
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
                return @"id,old_id,old_server,sm_name";
            }
        }

        public string sql_where
        {
            get
            {
                return string.Empty;
            }
        }

        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {
            
        }

        public void load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            while(rds.Read())
            {
                var id = rds.GetUInt64(0);
                var old_id = rds.GetUInt64(1);
                var old_server = rds.GetString(2);
                var name = rds.GetString(3);
                var ava = new AvatarInfo();
                ava.new_dbid = id;
                ava.old_dbid = old_id;
                ava.serverid = old_server;
                ava.old_name = name;
                dst.Add(ava);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
