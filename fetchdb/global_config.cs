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
    class global_config
    {
        public static int MYSQL_EXCUTE_MAX_TIMEOUT_SECOND = 300;
        public static readonly data.tbname avatar_table_name = new data.tbname(@"tbl_Avatar");
        public static readonly data.tbname account_table_name = new data.tbname(@"tbl_Account");
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
