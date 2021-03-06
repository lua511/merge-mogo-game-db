﻿/*
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
    class safeformat
    {
        public static string format(string fmt, params string[] obj)
        {
            return string.Format(fmt, obj);
        }
        public static string get_keystr_from_dbid_serverid(UInt64 old_dbid,string serverid)
        {
            return string.Format(@"{0}@{1}", old_dbid, serverid);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
