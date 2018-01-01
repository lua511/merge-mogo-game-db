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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace fetchdb.cpe_wraper
{
    class serialize_op<U> where U : class
    {
        public void save(string file_name,U obj)
        {
            using (var fs = new System.IO.FileStream(file_name, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
            }
        }
        public U load(string file_name)
        {
            using (var fs = new System.IO.FileStream(file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                var bf = new BinaryFormatter();
                var cinfo = bf.Deserialize(fs) as U;
                return cinfo;
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
