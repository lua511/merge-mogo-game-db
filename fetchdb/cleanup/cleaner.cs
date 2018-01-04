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

namespace fetchdb.cleanup
{
    class cleaner
    {
        public void main(workspace wspace)
        {
            var  files = System.IO.Directory.GetFiles(System.Environment.CurrentDirectory,"*.dat");
            foreach(var f in files)
            {
                System.IO.File.Delete(f);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */