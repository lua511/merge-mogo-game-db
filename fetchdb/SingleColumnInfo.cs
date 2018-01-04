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
    class SingleColumnInfoDesc
    {
        public string column_name { get; set; }
        public bool is_rawint { get; set; }
    }

    [Serializable]
    class SingleColumnInfo
    {
        public UInt64 dbid;
        public UInt64 int_value;
        public string str_value;
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */