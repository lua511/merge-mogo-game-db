/*
   +----------------------------------------------------------------------+
   | https://github.com/lua511/merge-mogo-game-db                         |
   +----------------------------------------------------------------------+
   | Author: winter yang <blueliuan@163.com>                              |
   +----------------------------------------------------------------------+  
*/

using fetchdb.cmd_wraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fetchdb
{
    class merge
    {
        private workspace wspace;

        // skip table that not generated from entity
        private bool is_table_need_skip(string table_name)
        {
            if(table_name.StartsWith("tbl_"))
            {
                return false;
            }
            return true;
        }

        public void main(workspace wspace)
        {           
            this.wspace = wspace;
            var cd = new cachedata();
            cachedata.Instance = cd;
            // ensure the target database exists
            new merge_op.target_dbop(data.database.Instance, cd).process(wspace);
            // process avatar table
            new merge_op.avatar_tblop(data.database.Instance, cd).process(wspace);
            // process account table
            new merge_op.account_tblop(data.database.Instance, cd).process(wspace);
            // rename avatar
            new merge_op.rename_avatar(data.database.Instance, cd).process(wspace);
            // remap account
            new merge_op.remap_account(data.database.Instance, cd).process(wspace);
            // simple copy some empty tables
            new merge_op.simplecopy_tblop(data.database.Instance, cd).process(wspace);
            // copy table & rename id
            new merge_op.remapid_tblop(data.database.Instance, cd).process(wspace);
            // simple one table
            new merge_op.simpleone_tblop(data.database.Instance, cd).process(wspace);
            // drop all tables
            new merge_op.simpledropall_tbop(data.database.Instance, cd).process(wspace);
            // rename id & name
            new merge_op.remap_idname_tblop(data.database.Instance, cd).process(wspace);
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
