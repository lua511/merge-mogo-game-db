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
    // todo : save worspace to disk
    [Serializable]
    class workspace
    {
        private string schema_db = string.Empty;

        public string Name { get; set; }
        public string TargetDb { get; set; }
        public List<string> SourceDbs
        {
            get;set;
        }
        public int  GetSourceDbIndex(string source_db)
        {
            for(int i = 0;i < SourceDbs.Count; ++i)
            {
                if(SourceDbs[i] == source_db)
                {
                    return i + 1;
                }
            }
            return SourceDbs.Count + 1;
        }
        public string SchemaDatabase
        {
            get
            {
                if(!string.IsNullOrEmpty(schema_db))
                {
                    return schema_db;
                }
                if (SourceDbs != null && SourceDbs.Count > 0)
                {
                    return SourceDbs[0];
                }
                return string.Empty;
            }
            set
            {
                schema_db = value;
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0
 */
