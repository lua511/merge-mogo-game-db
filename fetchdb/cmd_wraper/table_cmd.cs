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

namespace fetchdb.cmd_wraper
{
    class table_cmd
    {
        private database current_db;
        public table_cmd(database db)
        {
            current_db = db;
        }

        public List<string> show_tables(string database_name)
        {
            var ocs = new StringReaderAdaptor();
            current_db.execute_read(@"show tables in " + database_name, ocs.Load);

            return ocs.Values;
        }
        public string show_createtable(string database_name,string table_name)
        {
            var ocs = new StringReaderAdaptor(1);
            current_db.execute_read(@"use " + database_name + @";" + @"show create table " + table_name,ocs.Load);
            return ocs.Values.First();
        }
        public void drop_table(string database_name, string table_name)
        {
            var cmd_text = @"use " + database_name + @";" + @"drop table " + table_name;
            current_db.execute_nonquery(cmd_text);
        }

        public void create_table(string schema_str,string database_name)
        {
            current_db.execute_nonquery(@"use " + database_name + ";" + schema_str);
        }

        // copy table ( from show_create_table str to new db)
        public void copy_create_table_avatarlike(string schema_str,string database_name)
        {
            var tablename_hint = System.Text.RegularExpressions.Regex.Match(schema_str, @"CREATE TABLE\s\`[a-zA-Z_]+\`").Value;
            var table_name = tablename_hint.Split('`')[1];
            // reset the auto increate id
            var auto_inc_pattern = @"AUTO_INCREMENT\=[\d]+\s";
            var new_inc_pattern = @"AUTO_INCREMENT=" + UInt32.MaxValue.ToString() + @" ";
            schema_str = System.Text.RegularExpressions.Regex.Replace(schema_str, auto_inc_pattern, new_inc_pattern);
            // unique key to index
            schema_str = schema_str.Replace("UNIQUE KEY", "INDEX");

            // create table
            create_table(schema_str,database_name);
            // alter table add serverid/oldid
            var alter_sql = @"use " + database_name + @";" + @"ALTER TABLE " + table_name + @" ADD COLUMN old_server varchar(255) COLLATE utf8_bin DEFAULT NULL,ADD COLUMN old_id bigint(20) NOT NULL";
            current_db.execute_nonquery(alter_sql);
            var alter_sql_key = @"use " + database_name + @";" + @"ALTER TABLE " + table_name + @" ADD INDEX (old_server),ADD INDEX (old_id)";
            current_db.execute_nonquery(alter_sql_key);
        }

        // the table must have key(named id) value (named name) cols
        public Dictionary<UInt64,string>    load_key_name(string database_name,string table_name)
        {
            var ocs = new KeyStringReaderAdaptor();
            var cmd_text = @"select id,sm_name from " + database_name + @"." + table_name + @";";
            current_db.execute_read(cmd_text, ocs.Load);
            return ocs.KeyValues;
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
