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

namespace fetchdb
{
    [Serializable]
    class TableIdInfoDesc
    {
        public List<string> columns { get; set; }
    }

    [Serializable]
    class TableIdInfo
    {
        public UInt64 new_dbid { get; set; }
        public UInt64 old_dbid { set; get; }
        public string serverid { get; set; }
        public List<UInt64> values { get; set; }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
