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

namespace fetchdb.cpe_wraper
{
    interface iread_adaptor
    {
        string sql_select { get; }
        string sql_where { get; }
        void update(MySql.Data.MySqlClient.MySqlCommand cmd);
        void load(MySql.Data.MySqlClient.MySqlDataReader rds);
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
