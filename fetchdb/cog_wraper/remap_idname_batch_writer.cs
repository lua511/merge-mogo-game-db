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
    class remap_idname_batch_writer : cpe_wraper.iwrite_adaptor
    {
        private List<TableIdNameInfo> datas;
        private TableIdNameDesc desc;
        public remap_idname_batch_writer(TableIdNameDesc desc,List<TableIdNameInfo> info)
        {
            this.desc = desc;
            this.datas = info;
        }

        public string sql_set
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(desc.left_name + @" = CASE id");
                builder.AppendLine();
                for(int i = 0;i < datas.Count; ++i)
                {
                    var data = datas[i];
                    builder.Append(@"WHEN @id" + i.ToString());
                    builder.Append(@" THEN @" + desc.left_name + i.ToString());
                    builder.AppendLine();
                }
                builder.AppendLine(@"END,");

                builder.Append(desc.right_name + @" = CASE id");
                builder.AppendLine();
                for(int i = 0;i < datas.Count; ++i)
                {
                    var data = datas[i];
                    builder.Append(@"WHEN @id" + i.ToString());
                    builder.Append(@" THEN @" + desc.right_name + i.ToString());
                    builder.AppendLine();
                }
                builder.AppendLine(@"END");
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
            for(int i = 0;i < datas.Count; ++i)
            {
                var data = datas[i];
                cmd.Parameters.AddWithValue(@"@id" + i.ToString(), data.new_dbid);
                cmd.Parameters.AddWithValue(@"@" + desc.left_name + i.ToString(), data.record_id);
                cmd.Parameters.AddWithValue(@"@" + desc.right_name + i.ToString(), data.record_name);
            }
        }

    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */