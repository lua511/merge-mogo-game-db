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

namespace fetchdb.cpe_wraper
{
    class blobstring_loader
    {
        public string Load(MySql.Data.MySqlClient.MySqlDataReader myData,int my_idx)
        {
            int buff_len = 4096;
            var mem = new System.IO.MemoryStream();
            var buff = new byte[buff_len];
            int offset = 0;
            int readBytes = 0;

            while ((readBytes = (int)myData.GetBytes(my_idx, offset, buff, 0, buff_len)) > 0)
            {
                mem.Write(buff, 0, readBytes);
                if (readBytes < buff_len)
                {
                    break;
                }
                offset = readBytes;
            }
            mem.Position = 0;
            using (var sr = new System.IO.StreamReader(mem, Encoding.UTF8))
            {
                var line = sr.ReadToEnd();

                return line;
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
