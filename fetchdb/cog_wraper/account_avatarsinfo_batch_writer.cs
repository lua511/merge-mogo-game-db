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
    class account_avatarsinfo_batch_writer : cpe_wraper.iwrite_adaptor
    {
        private List<AccountInfo> accounts;
        public account_avatarsinfo_batch_writer(List<AccountInfo> accs)
        {
            accounts = accs;
        }

        public string sql_set
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(@"sm_avatarsInfo = CASE id");
                builder.AppendLine();
                int cnt = accounts.Count;
                for(var i = 0;i <cnt;++i)
                {
                    builder.Append(@"WHEN @id" + i.ToString());
                    builder.Append(@" THEN @sm_avatarsInfo" + i.ToString());
                    builder.AppendLine();
                }
                builder.AppendLine("END");
                return builder.ToString();
            }
        }

        public string sql_where
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(@"id IN (");
                int cnt = accounts.Count;
                for(var i = 0;i < cnt; ++i)
                {
                    if(i > 0)
                    {
                        builder.Append(@",");
                    }
                    builder.Append(@"@id" + i.ToString());
                }
                builder.Append(@")");
                return builder.ToString();
            }
        }

        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {
            int cnt = accounts.Count;
            for (int i = 0; i < cnt; ++i)
            {
                var acc = accounts[i];
                cmd.Parameters.AddWithValue(@"@id" + i.ToString(), acc.new_dbid);
                cmd.Parameters.AddWithValue(@"@sm_avatarsInfo" + i.ToString(), Encoding.UTF8.GetBytes(acc.new_avatarinfo));
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */