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
    class script_id_writer : cpe_wraper.iwrite_adaptor
    {
        private List<ScriptIdInfo> datas;
        private ScriptIdInfoDesc desc;

        public script_id_writer(ScriptIdInfoDesc desc,List<ScriptIdInfo> datas)
        {
            this.datas = datas;
            this.desc = desc;
        }

        public string sql_set
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(desc.column_name);
                builder.Append(@" = CASE id");
                builder.AppendLine();
                for(int i = 0;i < datas.Count; ++i)
                {
                    var data = datas[i];
                    builder.Append(@"WHEN @id" + i.ToString());
                    builder.Append(@" THEN @" + desc.column_name + i.ToString());
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
            int cnt = datas.Count;
            for(int i = 0;i < cnt; ++i)
            {
                var data = datas[i];
                var id = data.id_dbid;
                var val = data.new_info;
                cmd.Parameters.AddWithValue(@"@id" + i.ToString(), id);
                cmd.Parameters.AddWithValue(@"@" + desc.column_name + i.ToString(), Encoding.UTF8.GetBytes(val));
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */