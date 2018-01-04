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
    class avatar_name_batch_writer : cpe_wraper.iwrite_adaptor
    {
        private List<AvatarInfo> avatars;
        public avatar_name_batch_writer(List<AvatarInfo> avatars)
        {
            this.avatars = avatars;
        }
        public string sql_set
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(@"sm_name= CASE id");
                builder.AppendLine();
                int cnt = avatars.Count;
                for(var i = 0;i < cnt; ++i)
                {
                    builder.Append(@"WHEN @id" + i.ToString());
                    builder.Append(@" THEN @sm_name" + i.ToString());
                    builder.AppendLine();
                }
                builder.AppendLine(@"END");
                return builder.ToString();
            }
        }
        public string sql_where
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(@"id IN (");
                int cnt = avatars.Count;
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
            int cnt = avatars.Count;
            for(int i = 0;i < cnt; ++i)
            {
                var ava = avatars[i];
                cmd.Parameters.AddWithValue(@"@id" + i.ToString(), ava.new_dbid);
                cmd.Parameters.AddWithValue(@"@sm_name" + i.ToString(), ava.new_name);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */