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
    // typeinfo is better than just string ( think tbname & dbname )
    class tbname
    {
        private string str_table_name;

        public tbname(string name)
        {
            str_table_name = name;
        }

        public string table_name
        {
            get
            {
                return str_table_name;
            }
        }
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        private static tbname empty = new tbname(string.Empty);
        public static tbname Empty
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
