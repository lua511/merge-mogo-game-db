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
    class Program
    {
        static void Main(string[] args)
        {
            var db = new data.database();
            try
            {
                db.startup("192.168.1.8", "root", "123456");
                data.database.Instance = db;
                var wspace = new workspace();
                wspace.Name = "todo";
                wspace.TargetDb = new data.dbname("merged");
                wspace.SourceDbs = new List<data.dbname>()
                {
                    new data.dbname("card1"),
                    new data.dbname("card2"),
                    new data.dbname("card3")
                };

                new merge().main(wspace);
                Console.WriteLine("any key to exit");
                Console.ReadKey();
            }
            finally
            {
                db.cleanup();
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
