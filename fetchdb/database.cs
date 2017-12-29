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

namespace fetchdb
{
    // only connection manager & sql_cmd executer
    class database
    {
        public static database Instance
        {
            get;set;
        }

        public MySql.Data.MySqlClient.MySqlConnection Conn
        {
            get
            {
                return connection;
            }
        }

        #region mysql_raw
        private string connection_string;
        private MySql.Data.MySqlClient.MySqlConnection connection = null;

        public bool startup(string ip,string user,string password)
        {
            var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            builder.Server = ip;
            builder.UserID = user;
            builder.Password = password;
            builder.CharacterSet = "utf8";
            connection_string = builder.ToString();
            connection = new MySql.Data.MySqlClient.MySqlConnection(connection_string);
            try
            {
                connection.Open();
            }
            catch(System.Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                connection = null;
                return false;
            }
            return true;
        }

        public void cleanup()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        public void Ping()
        {
            if(connection != null)
            {
                connection.Ping();
                if (connection.State != System.Data.ConnectionState.Connecting)
                {
                    connection = new MySql.Data.MySqlClient.MySqlConnection(connection_string);

                    connection.Open();  // if open is fail,throw the exception
                }
            }
        }
        #endregion

        // mogo db need "tbl_" header
        private string get_full_table_name(string database_name,string table_name)
        {
            string header = "tbl_";
            return database_name + @"." + (table_name.StartsWith(header) ? "" : header) + table_name;
        }

        class one_col_string
        {
            private List<string> dst = new List<string>();

            public void Load(MySql.Data.MySqlClient.MySqlDataReader rds)
            {
                while (rds.Read())
                {
                    dst.Add(rds.GetString(0));
                }
            }

            public List<string> Dst
            {
                get { return dst; }
            }
        }

        #region simple_thin_wrapper

        // ExecuteReader
        public void    execute_read(string cmd_text,Action<MySql.Data.MySqlClient.MySqlDataReader> act)
        {
            var cmd = new MySql.Data.MySqlClient.MySqlCommand(cmd_text, connection);
            cmd.CommandTimeout = global_config.MYSQL_EXCUTE_MAX_TIMEOUT_SECOND;
            using (var reader = cmd.ExecuteReader())
            {
                act(reader);
            }
        }

        // ExecuteNonQuery
        public void execute_nonquery(string cmd_text)
        {
            var cmd = new MySql.Data.MySqlClient.MySqlCommand(cmd_text, connection);
            cmd.CommandTimeout = global_config.MYSQL_EXCUTE_MAX_TIMEOUT_SECOND;
            cmd.ExecuteNonQuery();
        }


        #region temp_functions

        public List<string> get_dump_names(string sql)
        {
            var ocs = new one_col_string();
            execute_read(sql, ocs.Load);

            return ocs.Dst;
        }

        public void run_sql_noquery(string sql)
        {
            execute_nonquery(sql);
        }

        #endregion

        #endregion
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
