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
            Console.WriteLine("hint: change mysql ip/database settings in the Program.cs first.(yes to continue,or else quit)");
            var  opt_str = Console.ReadLine();
            if (!opt_str.Trim().ToLower().StartsWith("y"))
            {
                Console.WriteLine("hit any key to exit");
                Console.ReadKey();

            }
            else
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

                    Console.WriteLine("menu:");
                    Console.WriteLine("  1: merge source dbs");
                    Console.WriteLine("  2: deep find all ids");
                    opt_str = Console.ReadLine().Trim().ToLower();
                    switch(opt_str)
                    {
                        case "1":
                            new merge().main(wspace);
                            break;
                        case "2":
                            new deepfind.check().main(wspace);
                            break;
                    }
                    Console.WriteLine("hit any key to exit");
                    Console.ReadKey();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("exception,(is mysql config right?),hit any key to exit");
                    Console.WriteLine("msg:" + ex.Message);
                    Console.ReadKey();
                }
                finally
                {
                    db.cleanup();
                }
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
