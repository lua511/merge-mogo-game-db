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

namespace fetchdb.deepfind
{
    class check_table
    {
        private data.database curr_conn;
        private data.tbname tbl_name;
        private data.dbname db_name;
        private diagram_rawdata profile;

        public check_table(data.database curr_conn, data.tbname tbl_name, data.dbname db_name, diagram_rawdata profile)
        {
            this.curr_conn = curr_conn;
            this.tbl_name = tbl_name;
            this.db_name = db_name;
            this.profile = profile;
        }

        public void search_table()
        {
            var cur_table = new cmd_wraper.table_cmd(curr_conn, tbl_name, db_name);
            var show_create = cur_table.show_createtable();

            var blob_pattern = @"\`[a-zA-Z_]{3,20}\`\slongblob";
            var rawint_pattern = @"\`sm_[a-zA-Z_]{1,20}\`\sbigint\(20\)";
            var name_pattern = @"\`[^\`]+\`";

            var blob_matches = System.Text.RegularExpressions.Regex.Matches(show_create, blob_pattern);
            foreach(System.Text.RegularExpressions.Match m in blob_matches)
            {
                var m2 = System.Text.RegularExpressions.Regex.Match(m.Value, name_pattern);
                var column_name = m2.Value;
                column_name = column_name.Substring(1, column_name.Length - 2);
                serach_column(column_name,false);
            }
            
            var rawint_matches = System.Text.RegularExpressions.Regex.Matches(show_create, rawint_pattern);
            foreach(System.Text.RegularExpressions.Match m in rawint_matches)
            {
                var m2 = System.Text.RegularExpressions.Regex.Match(m.Value, name_pattern);
                var column_name = m2.Value;
                column_name = column_name.Substring(1, column_name.Length - 2);
                serach_column(column_name,true);
            }
        }

        public void serach_column(string column_name,bool raw_int)
        {
            var scid = new SingleColumnInfoDesc();
            scid.is_rawint = raw_int;
            scid.column_name = column_name;

            var bdr = new cog_wraper.blobdata_reader(scid);
            new cmd_wraper.table_cmd(curr_conn, tbl_name, db_name).load_records(bdr);

            int match_threshhold = 10;

            var all_datas = bdr.Values;
            var matched_data = new List<SingleColumnInfo>();
            foreach(var v in all_datas)
            {
                if(raw_int)
                {
                    if(v.int_value > UInt32.MaxValue)
                    {
                        matched_data.Add(v);
                    }
                }
                else
                {
                    var matches = System.Text.RegularExpressions.Regex.Matches(v.str_value, @"\d{10,20}");
                    if(matches.Count > 0)
                    {
                        matched_data.Add(v);
                    }
                }
                if(matched_data.Count >= match_threshhold)
                {
                    profile.add(new uid_like_info() { column_name = column_name,raw_data = v.ToString(),table_name = tbl_name,database_name = db_name,sample_datas = matched_data });
                    break;
                }
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */