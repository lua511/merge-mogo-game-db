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
    class avatar_name_writer : cpe_wraper.iwrite_adaptor
    {
        private AvatarInfo current_avatar;
        public avatar_name_writer(AvatarInfo ainfo)
        {
            current_avatar = ainfo;
        }

        public string sql_set
        {
            get
            {
                var text = @"sm_name=@sm_name";
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
            cmd.Parameters.AddWithValue(@"@sm_name", current_avatar.new_name);
            cmd.Parameters.AddWithValue(@"@id", current_avatar.new_dbid);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */