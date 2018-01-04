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
    class blobdata_reader : cpe_wraper.iread_adaptor
    {
        private List<SingleColumnInfo> dst;
        private SingleColumnInfoDesc desc;
        public blobdata_reader(SingleColumnInfoDesc desc)
        {
            dst = new List<SingleColumnInfo>();
            this.desc = desc;
        }
        public List<SingleColumnInfo> Values
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
                return @"id," + desc.column_name;
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
                var sci = new SingleColumnInfo();
                sci.dbid = rds.GetUInt64(0);
                if(desc.is_rawint)
                {
                    sci.int_value = rds.GetUInt64(1);
                }
                else
                {
                    sci.str_value = new cpe_wraper.blobstring_loader().Load(rds, 1);
                }
                dst.Add(sci);
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
