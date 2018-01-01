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
    class account_avatarsinfo_writer : cpe_wraper.iwrite_adaptor
    {
        private AccountInfo curent_account;
        public account_avatarsinfo_writer(AccountInfo aci)
        {
            curent_account = aci;
        }

        public string sql_set
        {
            get
            {
                var text = @"sm_avatarsInfo = @sm_avatarsInfo";
                return text;
            }
        }

        public string sql_where
        {
            get
            {
                var text = @"id = @id";
                return text;
            }
        }

        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue(@"@sm_avatarsInfo", Encoding.UTF8.GetBytes(curent_account.new_avatarinfo));
            cmd.Parameters.AddWithValue(@"@id", curent_account.new_dbid);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */