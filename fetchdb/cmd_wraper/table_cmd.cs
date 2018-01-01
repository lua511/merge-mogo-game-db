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
    // methods:
    //    1. exists
    //    2. show create table
    //    3. drop table & safe drop table
    //    4. get table size ( element count )
    //    5. create table ( will only be used by cnl wrappers )
    //    6. alter table
    //    7. load records
    //    8. update record
    //    9. delete record
    // todo:
    //    move cmd operation to nl layer( og layer may ok,but not here )
    class table_cmd
    {
        private data.database current_conn;
        private string db_name;
        private string table_name;
        public table_cmd(data.database db,data.tbname table_name,data.dbname db_name)
        {
            current_conn = db;
            this.table_name = table_name.table_name;
            this.db_name = db_name.database_name;
        }
        // 1. exists
        public bool exists()
        {
            var tables = new database_cmd(current_conn,new data.dbname(db_name)).show_tables();
            return tables.Contains(table_name);
        }
        // 2. show create table
        public string show_createtable()
        {
            var ocs = new cpe_wraper.StringReaderAdaptor(1,"","");
            var cmd_text = safeformat.format(@"use {0};show create table {1}", db_name, table_name);
            var cmd = current_conn.prepare_read_cmd(cmd_text);
            current_conn.execute_read(cmd,ocs.load);
            return ocs.Values.First();
        }
        // 3. drop table & safe drop table
        public void drop_table(bool bSafe)
        {
            if(bSafe && !exists())
            {
                return;
            }

            var cmd_text = safeformat.format(@"use {0};drop table {1}", db_name, table_name);
            var cmd = current_conn.prepare_write_cmd(cmd_text);
            current_conn.execute_nonquery(cmd);
        }
        // 4.  get table size ( record count)
        public int get_record_count()
        {
            var ira = new cpe_wraper.IntReaderAdaptor(0,"","");
            var sql = safeformat.format(@"select count(1) from {0}.{1}", db_name, table_name);
            var cmd = current_conn.prepare_read_cmd(sql);
            current_conn.execute_read(cmd, ira.load);
            return ira.Values.First();
        }

        // 5.  create table (todo: use raw sql_cmd as less)
        public void create_table(string sql_text)
        {
            var txt = safeformat.format(@"use {0};{1}", db_name, sql_text);
            var cmd = current_conn.prepare_write_cmd(txt);
            current_conn.execute_nonquery(cmd);
        }

        // 6.  alter table ( thinking: more functions like add index / add column ...)
        public void alter_table(string sql_text)
        {
            var txt = safeformat.format(@"use {0}; ALTER TABLE {1} {2}", db_name, table_name, sql_text);
            var cmd = current_conn.prepare_write_cmd(txt);
            current_conn.execute_nonquery(cmd);
        }

        // 7. load records
        public void load_records(cpe_wraper.iread_adaptor reader)
        {
            var txt = "";
            if(string.IsNullOrEmpty(reader.sql_where))
            {
                txt = safeformat.format(@"select {0} from {1}.{2}",reader.sql_select,db_name,table_name);
            }
            else
            {
                txt = safeformat.format(@"select {0} from {1}.{2} where {3}", reader.sql_select, db_name, table_name, reader.sql_where);
            }
            var cmd = current_conn.prepare_read_cmd(txt);
            reader.update(cmd);
            current_conn.execute_read(cmd, reader.load);
        }

        // 8. update record
        public void update_record(cpe_wraper.iwrite_adaptor writer)
        {
            var txt = "";
            if (string.IsNullOrEmpty(writer.sql_where))
            {
                txt = safeformat.format(@"update {0}.{1} set {2}", db_name, table_name, writer.sql_set);
            }
            else
            {
                txt = safeformat.format(@"update {0}.{1} set {2} where {3}", db_name, table_name, writer.sql_set,writer.sql_where);
            }
            var cmd = current_conn.prepare_write_cmd(txt);
            writer.update(cmd);
            current_conn.execute_nonquery(cmd);
        }

        // 9. delete record
        public void delete_record(cpe_wraper.idelete_adaptor deletor)
        {
            var text = safeformat.format(@"delete from {0}.{1} where {2}", db_name, table_name, deletor.sql_where);
            var cmd = current_conn.prepare_write_cmd(text);
            deletor.update(cmd);
            current_conn.execute_nonquery(cmd);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
