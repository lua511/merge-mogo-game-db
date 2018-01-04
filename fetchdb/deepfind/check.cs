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
    class check
    {
        public void main(workspace wspace)
        {
            var dr = new diagram_rawdata();
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            foreach(var v in wspace.SourceDbs)
            {
                var tables = new cmd_wraper.database_cmd(data.database.Instance, v).show_tables();
                foreach(var t in tables)
                {
                    if(t.StartsWith(@"tbl_"))
                    {
                        var tbl = new data.tbname(t);
                        new check_table(data.database.Instance, tbl, v, dr).search_table();
                    }
                }
            }
            watch.Stop();
            Console.WriteLine("read database used seconds:" + watch.ElapsedMilliseconds / 1000);
            dr.save_to_disk("checked_tables.xml");
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
