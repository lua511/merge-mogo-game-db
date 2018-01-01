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
        private data.dbname schema_db = null;

        public string Name { get; set; }
        public data.dbname TargetDb { get; set; }
        public List<data.dbname> SourceDbs
        {
            get;set;
        }
        public int  GetSourceDbIndex(string source_db)
        {
            for(int i = 0;i < SourceDbs.Count; ++i)
            {
                if(SourceDbs[i].database_name == source_db)
                {
                    return i + 1;
                }
            }
            return SourceDbs.Count + 1;
        }
        public data.dbname SchemaDatabase
        {
            get
            {
                if(schema_db != null)
                {
                    return schema_db;
                }
                if (SourceDbs != null && SourceDbs.Count > 0)
                {
                    return SourceDbs[0];
                }
                throw new Exception("cannot find schema data");
            }
            set
            {
                schema_db = value;
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
