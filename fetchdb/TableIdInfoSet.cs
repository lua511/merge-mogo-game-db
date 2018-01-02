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
    class TableIdInfoSet
    {
        private TableIdInfoDesc table_desc;
        private List<TableIdInfo> all_records;

        public TableIdInfoDesc  Desc
        {
            get
            {
                return table_desc;
            }
            set
            {
                table_desc = value;
            }
        }
        public data.tbname table_name
        {
            get;
            set;
        }

        public List<TableIdInfo> Records
        {
            get
            {
                return all_records;
            }
            set
            {
                all_records = value;
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */