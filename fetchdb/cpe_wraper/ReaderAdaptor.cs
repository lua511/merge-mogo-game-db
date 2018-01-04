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

// todo : refactor 
namespace fetchdb.cpe_wraper
{

    class StringReaderAdaptor : iread_adaptor
    {
        private List<string> dst;
        private string str_sql_select;
        private string str_sql_where;
        private int data_index;

        public List<string> Values
        {
            get
            {
                return dst;
            }
        }

        public StringReaderAdaptor(int data_index,string sql_select,string sql_where)
        {
            dst = new List<string>();
            this.data_index = data_index;
            this.str_sql_select = sql_select;
            this.str_sql_where = sql_where;
        }

        public string sql_select
        {
            get
            {
                return str_sql_select;
            }
        }

        public string sql_where
        {
            get
            {
                return str_sql_where;
            }
        }

        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {

        }
        
        public void load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            while (rds.Read())
            {
                dst.Add(rds.GetString(data_index));
            }
        }
    }

    class IntReaderAdaptor : iread_adaptor
    {
        private List<int> dst;
        private int data_index;
        private string str_sql_select;
        private string str_sql_where;
        public List<int> Values
        {
            get
            {
                return dst;
            }
        }
        public IntReaderAdaptor(int data_index,string sql_select,string sql_where)
        {
            dst = new List<int>();
            this.data_index = data_index;
            this.str_sql_select = sql_select;
            this.str_sql_where = sql_where;
        }

        public string sql_select
        {
            get
            {
                return str_sql_select;
            }
        }
        
        public string sql_where
        {
            get
            {
                return str_sql_where;
            }
        }

        public void update(MySql.Data.MySqlClient.MySqlCommand cmd)
        {

        }

        public void load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            while(rds.Read())
            {
                dst.Add(rds.GetInt32(data_index));
            }
        }
    }


    class KeyStringReaderAdaptor
    {
        private Dictionary<UInt64, string> dst;
        public Dictionary<UInt64, string> KeyValues
        {
            get
            {
                return dst;
            }
        }

        public KeyStringReaderAdaptor()
        {
            dst = new Dictionary<ulong, string>();
        }

        public void Load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            while (rds.Read())
            {
                var key = rds.GetUInt64(0);
                var val = rds.GetString(1);
                dst.Add(key, val);
            }
        }
    }

    class KeyKeyStringReaderAdaptor
    {
        private Dictionary<UInt64, KeyValuePair<UInt64, string>> dst;
        public Dictionary<UInt64,KeyValuePair<UInt64,string>>   AllValues
        {
            get
            {
                return dst;
            }
        }
        public KeyKeyStringReaderAdaptor()
        {
            dst = new Dictionary<ulong, KeyValuePair<ulong, string>>();
        }
        public void Load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            while(rds.Read())
            {
                var key = rds.GetUInt64(0);
                var left = rds.GetUInt64(1);
                var right = rds.GetString(2);
                dst.Add(key, new KeyValuePair<ulong, string>(left, right));
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
