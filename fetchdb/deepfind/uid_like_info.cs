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

namespace fetchdb.deepfind
{
    class uid_like_info
    {
        public string column_name { get; set; }

        public string raw_data { get; set; }
        public data.tbname table_name { get; set; }
        public data.dbname database_name { get; set; }

        public List<SingleColumnInfo> sample_datas { get; set; }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */