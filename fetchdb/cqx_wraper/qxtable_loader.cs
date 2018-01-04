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

namespace fetchdb.cqx_wraper
{
    // the xml file has simple table node,load to list
    class qxtable_loader
    {
        // xml file has table node just under root
        public List<string> load_by_table(string file_name)
        {
            var dst = new List<string>();
            var doc = XDocument.Load(file_name);
            var nodes = from p in doc.Descendants("table") where p.Parent != null && p.Parent.Name == @"root" select p.Value;
            foreach(var n in nodes)
            {
                var val = @"tbl_" + n;
                dst.Add(val);
            }
            return dst;
        }

        // xml file : root \ table \ element
        public Dictionary<string,List<string>>  load_by_element(string file_name)
        {
            var dst = new Dictionary<string, List<string>>();
            var doc = XDocument.Load(file_name);
            var nodes = from p in doc.Descendants("element") where p.Parent != null && p.Parent.Parent != null && p.Parent.Parent.Name == @"root" select p;
            foreach(var n in nodes)
            {
                var val = n.Value;
                var ele = n.Parent.Name.ToString();
                val = @"sm_" + val;
                ele = @"tbl_" + ele;
                if(!dst.ContainsKey(ele))
                {
                    dst.Add(ele, new List<string>());
                }
                dst[ele].Add(val);
            }
            return dst;
        }
        // xml file : root \ table \ id-name
        public Dictionary<string,KeyValuePair<string,string>>   load_by_pair(string file_name)
        {
            var dst = new Dictionary<string, KeyValuePair<string, string>>();
            var doc = XDocument.Load(file_name);
            var nodes = from p in doc.Descendants("uint") where p.Parent != null && p.Parent.Parent != null && p.Parent.Parent.Name == "root" select p.Parent;
            foreach(var n in nodes)
            {
                var p1 = n.Descendants("uint").First();
                var p2 = n.Descendants("str").First();

                var key = @"tbl_" + n.Name.ToString();
                var v1 = @"sm_" + p1.Value;
                var v2 = @"sm_" + p2.Value;
                dst.Add(key, new KeyValuePair<string, string>(v1,v2));
            }
            return dst;
        }
        // xml file : root \ table \ column-name
        public List<KeyValuePair<string,string>> load_by_script_id(string file_name)
        {
            var dst = new List<KeyValuePair<string, string>>();
            var doc = XDocument.Load(file_name);
            var nodes = from p in doc.Descendants("column") where p.Parent != null && p.Parent.Parent != null && p.Parent.Parent.Name == @"root" select p;
            foreach(var n in nodes)
            {
                var column = @"sm_" + n.Value;
                var table = @"tbl_" + n.Parent.Name.ToString();
                dst.Add(new KeyValuePair<string, string>(table, column));
            }
            return dst;
        }
        public List<KeyValuePair<string,string>>    load_pattern_by_script_id(string file_name,string table_name,string column_name)
        {
            table_name = table_name.Replace(@"tbl_","");
            column_name = column_name.Replace(@"sm_","");
            var dst = new List<KeyValuePair<string, string>>();
            var doc = XDocument.Load(file_name);
            var nodes = from p in doc.Descendants("column") where p.Parent != null && p.Parent.Parent != null && p.Parent.Name == table_name && p.Value == column_name select p.Parent;
            var patterns = nodes.First().Descendants(@"pattern");
            foreach(var p in patterns)
            {
                var p1 = p.Value;
                var p2 = p.Attribute("exact").Value;
                dst.Add(new KeyValuePair<string, string>(p1, p2));
            }
            return dst;
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
