using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HuaBanPhotoDownload
{
    class HuabanUtil
    {
        static Regex regexTitle = new Regex(@"<title>(?<title>[\s\S]*?)收集");//group['title']
        //在title中找
        static Regex regexInTitle = new Regex(@"\((?<count>\d+)图\)_@(?<username>[\s\S]*)");

        //在pin_string里面找
        static Regex regexPinId = new Regex(@"""pin_id""[\s]*?:[\s]*?(?<id>\d+)");
        static Regex regexPinBucket = new Regex(@"""bucket"":""(?<bucket>\w+)""");
        static Regex regexPinKey = new Regex(@"""key"":""(?<key>[\w_-]+)""");
        static Regex regexPinType = new Regex(@"""type"":""image/(?<type>\w+)""");

        //找出board的title,username,count
        public static string FindTitle(string html)
        {
            return regexTitle.Match(html).Groups["title"].Value;
        }
        internal static string FindUsername(string title)
        {
            return regexInTitle.Match(title).Groups["username"].Value;
        }
        internal static string FindCount(string title)
        {
            return regexInTitle.Match(title).Groups["count"].Value;
        }


        /*
            img_host = { 
                "hbimg": "img.hb.aicdn.com",
                "hbfile": "hbfile.b0.upaiyun.com/img/apps" 
            }
            hbfile = {
                    "hbfile": "hbfile.b0.upaiyun.com", 
                    "hbimg2": "hbimg2.b0.upaiyun.com"
            }
         */
        //图片服务器
        static Dictionary<string, string> imgHost = new Dictionary<string, string>() { 
            { "hbimg", "img.hb.aicdn.com" },
            { "hbfile", "hbfile.b0.upaiyun.com/img/apps" }
        };
        static Dictionary<string, string> hbFile = new Dictionary<string, string>() { 
            { "hbfile", "hbfile.b0.upaiyun.com" },
            { "hbimg2", "hbimg2.b0.upaiyun.com" }
        };

        //返回[(int id,string src,string "image/jpeg")]
        internal static IEnumerable<Tuple<string, string, string>> FindPins(string html)
        {
            var pins_index = html.IndexOf("\"pins\"");  //"pins":[{"pin_id
            var remain = html.Substring(pins_index + 7); //[{...
            var end_index = Magicdawn.Util.StringFinder.GetSecondIndex(remain);
            remain = remain.Substring(0, end_index); // [...]

            var pins_string = new List<string>();
            while (remain.IndexOf('{') > 0)
            {
                var left = remain.IndexOf('{');
                var right = Magicdawn.Util.StringFinder.GetSecondIndex(remain, left);

                var content = remain.Substring(left + 1, right - left);//不包括 {}
                pins_string.Add(content);

                remain = remain.Substring(right);
            }

            foreach (var p_string in pins_string)
            {
                var id = regexPinId.Match(p_string).Groups["id"].Value;
                var bucket = regexPinBucket.Match(p_string).Groups["bucket"].Value;
                var key = regexPinKey.Match(p_string).Groups["key"].Value;
                var typeMatch = regexPinType.Match(p_string);

                var baseUrl = imgHost[bucket];
                var src = string.Format("http://{0}/{1}", baseUrl, key);

                var ext = "jpg";
                if (typeMatch != null)
                {
                    //type可能匹配不到
                    ext = GetFileExt(typeMatch.Groups["type"].Value);
                }
                yield return Tuple.Create(id, src, ext);
            }
        }

        internal static string GetFileExt(string type)
        {
            //type是image/xxx
            type = type.ToLowerInvariant();
            if (type == "jpeg" || type == "pjpeg")
                return "jpg";
            else
                return type;
        }


        public static bool Download(WebClient client, string src, string path, int times = 0)
        {
            try
            {
                client.DownloadFile(src, path);
            }
            catch (WebException)
            {
                times++;
                if (times <= Config.MaxTryTimes)
                {
                    return Download(client, src, path, times); //尝试下一次
                }
                else
                {
                    return false; //下载失败
                }
            }
            return true; //默认成功
        }

    }
}
