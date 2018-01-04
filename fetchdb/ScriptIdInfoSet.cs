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
    class ScriptIdInfoSet
    {
        public data.tbname table_name;
        public ScriptIdInfoDesc Desc;
        public List<ScriptIdInfo> Records { get; set; }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */