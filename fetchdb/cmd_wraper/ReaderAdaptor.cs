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


    class StringReaderAdaptor
    {
        private List<string> dst;
        private int data_index;

        public List<string> Values
        {
            get
            {
                return dst;
            }
        }

        public StringReaderAdaptor(int data_index)
        {
            dst = new List<string>();
            this.data_index = data_index;
        }
        public StringReaderAdaptor()
            : this(0)
        {

        }
        
        public void Load(MySql.Data.MySqlClient.MySqlDataReader rds)
        {
            while (rds.Read())
            {
                dst.Add(rds.GetString(data_index));
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
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
