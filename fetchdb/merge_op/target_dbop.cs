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

namespace fetchdb.merge_op
{
    // 1. check target db exists
    // 2. create it if need
    class target_dbop
    {
        private data.database current_connection;
        public target_dbop(data.database connection, cachedata cdata)
        {
            current_connection = connection;
        }

        public void process(workspace wspace)
        {
            var cmd = new cmd_wraper.database_cmd(current_connection, wspace.TargetDb);
            cmd.create_database(false);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */