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
using System.Xml.Linq;

namespace fetchdb.deepfind
{
    class diagram_rawdata
    {
        public diagram_rawdata()
        {
            confirmed_columns = new List<uid_like_info>();
        }
        public List<uid_like_info> confirmed_columns
        {
            get;
        }
        public void add(uid_like_info uli)
        {
            bool bexist = false;
            foreach(var v in confirmed_columns)
            {
                if(v.column_name == uli.column_name && v.table_name.table_name == uli.table_name.table_name)
                {
                    bexist = true;
                }
            }
            if(!bexist)
            {
                confirmed_columns.Add(uli);
            }
        }

        public void save_to_disk(string file_name)
        {
            var doc = new XDocument();
            var root = new XElement("root");

            var dst = new Dictionary<string, HashSet<string>>();
            foreach(var v in confirmed_columns)
            {
                if(!dst.ContainsKey(v.table_name.table_name))
                {
                    dst.Add(v.table_name.table_name, new HashSet<string>());
                }
                var collection = dst[v.table_name.table_name];
                if(!collection.Contains(v.column_name))
                {
                    collection.Add(v.column_name);
                    var node = new XElement("table", v.table_name.table_name);
                    node.Add(new XElement("col", v.column_name));

                    var smp = new XElement("samples");
                    foreach(var s in v.sample_datas)
                    {
                        var data_val = (s.int_value >= UInt32.MaxValue) ? s.int_value.ToString() : s.str_value;
                        var xe = new XElement("data", data_val);
                        xe.Add(new XAttribute("dbid", s.dbid));
                        smp.Add(new XElement("sample", xe));
                    }
                    node.Add(smp);
                    root.Add(node);
                }
            }
            doc.Add(root);
            doc.Save(file_name);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */