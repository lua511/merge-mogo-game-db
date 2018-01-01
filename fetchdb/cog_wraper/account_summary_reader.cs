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
    class account_summary_reader : cpe_wraper.iread_adaptor
    {
        private List<AccountInfo> dst = new List<AccountInfo>();
        public account_summary_reader()
        {
            dst = new List<AccountInfo>();
        }

        public List<AccountInfo> Values
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
                return @"id,sm_name,old_id,old_server,sm_avatarsInfo";
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
                var old_name = rds.GetString(1);
                var old_id = rds.GetUInt64(2);
                var serverid = rds.GetString(3);
                var old_avatarInfos = new cpe_wraper.blobstring_loader().Load(rds, 4);

                var acc = new AccountInfo();
                acc.new_dbid = id;
                acc.old_name = old_name;
                acc.old_dbid = old_id;
                acc.serverid = serverid;
                acc.old_avatarinfo = old_avatarInfos;
                dst.Add(acc);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
