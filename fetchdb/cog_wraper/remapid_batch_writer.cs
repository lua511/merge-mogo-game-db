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
    class remapid_batch_writer : cpe_wraper.iwrite_adaptor
    {
        private List<TableIdInfo> datas;
        private TableIdInfoDesc desc;
        public remapid_batch_writer(TableIdInfoDesc desc,List<TableIdInfo> datas)
        {
            this.desc = desc;
            this.datas = datas;
        }

        public string sql_set
        {
            get
            {
                var builder = new StringBuilder();
                for(int n = 0; n < desc.columns.Count; ++n)
                {
                    if(n != 0)
                    {
                        builder.Append(@",");
                    }
                    var column_name = desc.columns[n];
                    builder.Append(column_name + @" = CASE id");
                    builder.AppendLine();
                    for(int i = 0;i < datas.Count; ++i)
                    {
                        var data = datas[i];                        
                        builder.Append(@"WHEN @id" + i.ToString());
                        builder.Append(@" THEN @" + column_name + i.ToString());
                        builder.AppendLine();
                    }
                    builder.AppendLine(@"END");
                }
                return builder.ToString();
            }
        }
        public string sql_where
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(@"id IN (");
                for(int i = 0;i < datas.Count; ++i)
                {
                    if(i != 0)
                    {
                        builder.Append(@",");
                    }
                    builder.Append(@"@id" + i.ToString());
                }
                builder.Append(@")");
                return builder.ToString();
            }
        }

        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {
            int cnt = datas.Count;
            for(int i = 0;i < cnt; ++i)
            {
                var v = datas[i].new_dbid;
                cmd.Parameters.AddWithValue(@"id" + i.ToString(), v);
            }
            for(int n = 0; n < desc.columns.Count; ++n)
            {
                for (int i = 0; i < datas.Count; ++i)
                {
                    var column_name = desc.columns[n];
                    cmd.Parameters.AddWithValue(@"@" + column_name + i.ToString(), datas[i].values[n]);
                }
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
