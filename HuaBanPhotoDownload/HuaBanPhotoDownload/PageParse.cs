using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HuaBanPhotoDownload
{
    public class PageParse
    {
        /// <summary>
        /// 将整个页面图片的div域解析出来
        /// </summary>
        /// <param name="Source">动态页面源码</param>
        /// <returns>每张图片的div域的链表</returns>
        public static List<string> DivParse(string Source)
        {
            List<string> ImageRegion = new List<string>();
            Regex Reg = new Regex("");
            MatchCollection MC = Reg.Matches(Source);
            foreach(Match M in MC)
            {
                ImageRegion.Add(M.Groups[0].Value);
            }

            return ImageRegion;
        }

        /// <summary>
        /// 解析图片的div域里的信息
        /// </summary>
        /// <param name="ImageRegion">div域源码</param>
        /// <returns>返回字符串数组，0为图片id,1为图片名称</returns>
        public static string[] ImageInfoParse(string ImageRegion)
        {
            return null;
        }
        
    }
}
