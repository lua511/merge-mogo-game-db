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

// cnl:
//    1. cmd
//       cnl   the two brother will live together in the solution explorer
//    2. cnl next layer
//       it is not native mysql operation but a higher layer wrapper
namespace fetchdb.cnl_wrapper
{
    // 1. create a mirror table as merge destination ( add: old_id/old_server columns )
    // 2. fill data to the mirrored table
    // 3. clone a table ( without any addition column )
    class nltable_cmd
    {
        private data.database conn;
        private data.dbname database_name;
        private data.tbname table_name;

        public nltable_cmd(data.database conn,data.dbname dbname,data.tbname tblname)
        {
            this.conn = conn;
            this.database_name = dbname;
            this.table_name = tblname;
        }
        // 1. create a mirror table
        public void copy_from(data.dbname src_database_name)
        {
            var schema_str = new cmd_wraper.table_cmd(conn, table_name, src_database_name).show_createtable();
            // reset the auto increate id
            var auto_inc_pattern = @"AUTO_INCREMENT\=[\d]+\s";
            var new_inc_pattern = @"AUTO_INCREMENT=" + UInt32.MaxValue.ToString() + @" ";
            schema_str = System.Text.RegularExpressions.Regex.Replace(schema_str, auto_inc_pattern, new_inc_pattern);
            // unique key to index
            schema_str = schema_str.Replace("UNIQUE KEY", "INDEX");
            // create table
            var target_table = new cmd_wraper.table_cmd(conn, table_name, database_name);
            target_table.create_table(schema_str);
            // alter table add serverid/oldid
            target_table.alter_table(@"ADD COLUMN old_server varchar(255) COLLATE utf8_bin DEFAULT NULL,ADD COLUMN old_id bigint(20) NOT NULL");
            target_table.alter_table(@"ADD INDEX (old_server),ADD INDEX (old_id)");
        }
        // 2. fill data from src database
        public void fill_from(data.dbname src_database_name)
        {
            var schema = new cmd_wraper.table_cmd(conn, table_name, src_database_name).show_createtable();
            var columns_match = System.Text.RegularExpressions.Regex.Matches(schema, @"\n\s+\`[a-zA-Z_]+\`\s");
            List<string> usefull_columns = new List<string>();
            foreach (System.Text.RegularExpressions.Match m in columns_match)
            {
                var v = m.Value.Split('`');
                var name = v[1];
                if (name == "id")
                {
                    continue;
                }
                usefull_columns.Add(name);
            }
            var init_sql_schema = @"insert into {0}.{1} (old_id,old_server,{2}) select id as old_id,'{3}'as old_server, {2} from {3}.{1};";
            var column_strs = new StringBuilder();
            foreach (var v in usefull_columns)
            {
                column_strs.Append(v);
                column_strs.Append(",");
            }
            column_strs.Remove(column_strs.Length - 1, 1);

            var init_sql = safeformat.format(init_sql_schema, database_name.database_name, table_name.table_name, column_strs.ToString(), src_database_name.database_name);
            var cmd = conn.prepare_write_cmd(init_sql);
            new cmd_wraper.connection_cmd(conn).run_sql_text(init_sql);
        }
        // 3. clone a table
        public void clone_from(data.dbname src_database_name,int expected_cnt)
        {
            var schema_str = new cmd_wraper.table_cmd(conn, table_name, src_database_name).show_createtable();
            // reset the auto increate id
            var auto_inc_pattern = @"AUTO_INCREMENT\=[\d]+\s";
            var new_inc_pattern = @"AUTO_INCREMENT=" + UInt32.MaxValue.ToString() + @" ";
            schema_str = System.Text.RegularExpressions.Regex.Replace(schema_str, auto_inc_pattern, new_inc_pattern);

            var cnt = new cmd_wraper.table_cmd(conn, table_name, src_database_name).get_record_count();
            if(expected_cnt >= 0 && cnt != expected_cnt)
            {
                throw new Exception("not empty table," + table_name.table_name);
            }
            var target_table = new cmd_wraper.table_cmd(conn, table_name, database_name);
            target_table.create_table(schema_str);
        }
        public void clone_from(data.dbname src_database_name)
        {
            clone_from(src_database_name, 0);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
