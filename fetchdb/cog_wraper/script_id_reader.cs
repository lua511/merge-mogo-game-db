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
using MySql.Data.MySqlClient;

namespace fetchdb.cog_wraper
{
    class script_id_reader : cpe_wraper.iread_adaptor
    {
        private List<ScriptIdInfo> dst;
        private ScriptIdInfoDesc desc;

        public script_id_reader(ScriptIdInfoDesc desc)
        {
            dst = new List<ScriptIdInfo>();
            this.desc = desc;
        }
        public List<ScriptIdInfo>   Values
        {
            get
            {
                return dst;
            }
        }
        public string sql_select
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(@"id");
                builder.Append(@",old_id");
                builder.Append(@",old_server");
                builder.Append(@",").Append(desc.column_name);
                return builder.ToString();
            }
        }
        public string sql_where
        {
            get
            {
                return string.Empty;
            }
        }

        public void update(MySqlCommand cmd)
        {
        }

        public void load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            while(rds.Read())
            {
                var sii = new ScriptIdInfo();
                sii.id_dbid = rds.GetUInt64(0);
                sii.old_dbid = rds.GetUInt64(1);
                sii.serverid = rds.GetString(2);
                sii.old_info = rds.GetString(3);
                dst.Add(sii);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */