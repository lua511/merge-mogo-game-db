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

namespace fetchdb.data
{
    [Serializable]
    public class AvatarInfo
    {
        public string old_name { get; set; }
        public string new_name { get; set; }
        public UInt64 old_dbid { get; set; }
        public UInt64 new_dbid { get; set; }
        public string serverid { get; set; }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
