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
    class TableIdNameDesc
    {
        public string left_name { get; set; }
        public string right_name { get; set; }
    }
    [Serializable]
    class TableIdNameInfo
    {
        public UInt64 new_dbid { get; set; }
        public UInt64 old_dbid { get; set; }
        public string serverid { get; set; }
        
        public UInt64 record_id { get; set; }
        public string record_name { get; set; }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */