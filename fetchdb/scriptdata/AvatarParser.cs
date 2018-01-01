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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// todo : refactor
namespace fetchdb.scriptdata
{
    class AvatarHelper
    {
        public static string GetRightValue(string val)
        {
            var ele = val.Split('=');
            if (ele.Length != 2)
            {
                throw new Exception("not enough count");
            }
            var raw_text = ele[1].Trim();

            if (raw_text.StartsWith("s"))
            {
                return raw_text.Substring(4);
            }
            else
            {
                return raw_text;
            }
        }
    }

    class AvatarInfo : IComparable<AvatarInfo>
    {
        public UInt64 id;
        public string name;
        public int vocation;
        public int level;
        public string var5;
        public string var6;
        public string var7;
        public string var8;
        public string var9;
        public string var10;
        public string var11;
        public string var12;

        public bool is_empty = false;

        public int CompareTo(AvatarInfo other)
        {
            if (level > other.level)
            {
                return 1;
            }
            if (level < other.level)
            {
                return -1;
            }
            return 0;
        }
    }

    class AvatarsInfo
    {
        private List<AvatarInfo> all_avatar = new List<AvatarInfo>();

        public List<AvatarInfo> AllAvatars
        {
            get
            {
                return all_avatar;
            }
        }

        public void LoadFromString(string info)
        {
            var pattern = @"\{[^{}]+\}";
            var matches = Regex.Matches(info, pattern);
            foreach (Match ma in matches)
            {
                var val = ma.Value;
                if (!string.IsNullOrEmpty(val))
                {
                    // 直接拆分比较安全
                    var raw_text = val.Replace("{", "").Replace("}", "").Trim();
                    var leftright = raw_text.Split(',');
                    if (leftright.Length != 12)
                    {
                        throw new Exception("wrong avatar format");
                    }
                    var avatar = new AvatarInfo();
                    avatar.id = UInt64.Parse(AvatarHelper.GetRightValue(leftright[0]));
                    avatar.name = AvatarHelper.GetRightValue(leftright[1]);
                    avatar.vocation = int.Parse(AvatarHelper.GetRightValue(leftright[2]));
                    avatar.level = int.Parse(AvatarHelper.GetRightValue(leftright[3]));
                    avatar.var5 = AvatarHelper.GetRightValue(leftright[4]);
                    avatar.var6 = AvatarHelper.GetRightValue(leftright[5]);
                    avatar.var7 = AvatarHelper.GetRightValue(leftright[6]);
                    avatar.var8 = AvatarHelper.GetRightValue(leftright[7]);
                    avatar.var9 = AvatarHelper.GetRightValue(leftright[8]);
                    avatar.var10 = AvatarHelper.GetRightValue(leftright[9]);
                    avatar.var11 = AvatarHelper.GetRightValue(leftright[10]);
                    avatar.var12 = AvatarHelper.GetRightValue(leftright[11]);
                    all_avatar.Add(avatar);
                    all_avatar.Sort();
                }
            }
        }

        public string SaveToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            int real_count = 0;
            for (int i = 0; i < all_avatar.Count; ++i)
            {
                if (real_count >= 4)
                {
                    break;
                }
                var avatar = all_avatar[i];
                if(avatar.is_empty == true)
                {
                    continue;
                }
                ++real_count;
                int cnt = real_count;
                sb.Append(cnt.ToString());
                sb.Append("=");
                sb.Append("{");
                // 
                sb.Append("1=");
                sb.Append(avatar.id);
                sb.Append(",");
                sb.Append("2=");
                var len = Encoding.UTF8.GetBytes(avatar.name).Length;
                var name = string.Format("s{0:D3}{1}", len, avatar.name);
                sb.Append(name);
                sb.Append(",");
                sb.Append("3=");
                sb.Append(avatar.vocation);
                sb.Append(",");
                sb.Append("4=");
                sb.Append(avatar.level);

                sb.Append(",5=");
                sb.Append(avatar.var5);
                sb.Append(",6=");
                sb.Append(avatar.var6);
                sb.Append(",7=");
                sb.Append(avatar.var7);
                sb.Append(",8=");
                sb.Append(avatar.var8);
                sb.Append(",9=");
                sb.Append(avatar.var9);
                sb.Append(",10=");
                sb.Append(avatar.var10);
                sb.Append(",11=");
                sb.Append(avatar.var11);
                sb.Append(",12=");
                sb.Append(avatar.var12);
                //
                sb.Append("},");
            }
            if (sb.ToString().Last() == ',')
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("}");
            return sb.ToString();
        }

        public void Merge(AvatarsInfo rh)
        {
            foreach (var v in rh.all_avatar)
            {
                all_avatar.Add(v);
                all_avatar.Sort();
            }
        }
    }
}

/*
 * by Microsoft Visual Studio Community 2017 & NuGet 4.5.0 & .NET Framework 4.6.1
 */
