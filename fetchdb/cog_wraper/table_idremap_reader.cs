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

namespace fetchdb.cog_wraper
{
    class table_idremap_reader : cpe_wraper.iread_adaptor
    {
        private List<TableIdInfo> dst;
        private TableIdInfoDesc desc;
        public table_idremap_reader(TableIdInfoDesc desc)
        {
            this.desc = desc;
            dst = new List<TableIdInfo>();
        }

        public List<TableIdInfo> Values
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
                foreach (var v in desc.columns)
                {
                    builder.Append(@",");
                    builder.Append(v);
                }
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

        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {

        }

        public void load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            while (rds.Read())
            {
                var id = rds.GetUInt64(0);
                var ti = new TableIdInfo();
                ti.values = new List<ulong>();
                ti.new_dbid = id;
                ti.old_dbid = rds.GetUInt64(1);
                ti.serverid = rds.GetString(2);
                for (int i = 0; i < desc.columns.Count; ++i)
                {
                    var v = rds.GetUInt64(i + 3);
                    ti.values.Add(v);
                }
                dst.Add(ti);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */