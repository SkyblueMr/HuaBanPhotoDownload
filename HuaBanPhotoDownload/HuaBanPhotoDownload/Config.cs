using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaBanPhotoDownload
{
    class Config
    {
        //示例url
        public static readonly string ExampleUrl = "http://huaban.com/boards/13715778/";
        //是否debug
        public static readonly bool Debug;
        //文件
        public static readonly string ImageDir;
        //重试次数
        public static readonly int MaxTryTimes;
        //错误记录
        public static readonly string ErrorLog = "下载失败记录.txt";
        //线程数量
        public static int ThreadCount; //可通过命令行修改

        static string AppConfig(string key)
        {
            //如果删除config,返回null
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
         
        static Config()
        {
            Debug = bool.Parse(AppConfig("Debug") ?? "false");
            ImageDir = AppConfig("ImageDir") ?? "image";
            MaxTryTimes = int.Parse(AppConfig("MaxTryTimes") ?? "5");
            ThreadCount = int.Parse(AppConfig("ThreadCount") ?? "5");
        }

    }
}
