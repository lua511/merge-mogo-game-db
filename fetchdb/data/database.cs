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

namespace fetchdb.data
{
    // only connection manager & sql_cmd executer
    class database
    {
        public static database Instance
        {
            get;set;
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
            // no need surround with try-catch, !!! you catch it
            connection.Open();

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

        #region simple_thin_wrapper

        // ExecuteReader
        public MySql.Data.MySqlClient.MySqlCommand prepare_read_cmd(string cmd_text)
        {
            var cmd = new MySql.Data.MySqlClient.MySqlCommand(cmd_text, connection);
            cmd.CommandTimeout = global_config.MYSQL_EXCUTE_MAX_TIMEOUT_SECOND;
            return cmd;
        }

        public void execute_read(MySql.Data.MySqlClient.MySqlCommand cmd, Action<MySql.Data.MySqlClient.MySqlDataReader> act)
        {
            cmd.CommandTimeout = global_config.MYSQL_EXCUTE_MAX_TIMEOUT_SECOND;
            using (var reader = cmd.ExecuteReader())
            {
                act(reader);
            }
        }

        // ExecuteNonQuery
        public MySql.Data.MySqlClient.MySqlCommand prepare_write_cmd(string cmd_text)
        {
            var cmd = new MySql.Data.MySqlClient.MySqlCommand(cmd_text, connection);
            cmd.CommandTimeout = global_config.MYSQL_EXCUTE_MAX_TIMEOUT_SECOND;
            return cmd;
        }
        public void execute_nonquery(MySql.Data.MySqlClient.MySqlCommand cmd)
        {
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
