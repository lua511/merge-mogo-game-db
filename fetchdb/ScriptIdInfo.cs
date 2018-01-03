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
    class ScriptIdInfoDesc
    {
        public string column_name;
    }
    [Serializable]
    class ScriptIdInfo
    {
        public UInt64 id_dbid { get; set; }
        public UInt64 old_dbid { get; set; }
        public string serverid { get; set; }
        public string old_info { get; set; }
        public string new_info { get; set; }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */