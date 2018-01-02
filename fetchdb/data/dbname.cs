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
    class dbname
    {
        public string str_database_name;
        public dbname(string name)
        {
            str_database_name = name;
        }

        public string database_name
        {
            get
            {
                return str_database_name;
            }
        }
        [NonSerialized]
        private static dbname empty = new dbname(string.Empty);
        public static dbname Empty
        {
            get
            {
                return empty;
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
