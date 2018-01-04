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
    class table_idname_remap_reader : cpe_wraper.iread_adaptor
    {
        private List<TableIdNameInfo> dst;
        private TableIdNameDesc desc;
        public table_idname_remap_reader(TableIdNameDesc desc)
        {
            dst = new List<TableIdNameInfo>();
            this.desc = desc;
        }

        public List<TableIdNameInfo> Values
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
                builder.Append(@",").Append(desc.left_name);
                builder.Append(@",").Append(desc.right_name);
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
            while(rds.Read())
            {
                var id = rds.GetUInt64(0);
                var tin = new TableIdNameInfo();
                tin.new_dbid = id;
                tin.old_dbid = rds.GetUInt64(1);
                tin.serverid = rds.GetString(2);

                tin.record_id = rds.GetUInt64(3);
                tin.record_name = rds.GetString(4);

                dst.Add(tin);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */