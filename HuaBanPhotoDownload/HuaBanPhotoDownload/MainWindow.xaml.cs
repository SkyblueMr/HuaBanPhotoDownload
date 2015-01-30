using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.Windows.Threading;

namespace HuaBanPhotoDownload
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool CheckAnyRunning(Thread[] threads)
        {
            foreach (var t in threads)
            {
                if (t.IsAlive)
                {
                    return true;
                }
            }
            return false;
        }
        static void WriteWithColor(Action act)
        {
            var old = Console.ForegroundColor;
            act();
            Console.ForegroundColor = old;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<string> UrlList = new List<string>();
        private bool IsRun = true;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Thread apps = new Thread(new ParameterizedThreadStart((object s) => 
            {
                while (IsRun)
                {
                    Thread.Sleep(200);
                    IDataObject data = Clipboard.GetDataObject();
                    if (data.GetDataPresent(DataFormats.Text))
                    {
                        string ul = data.GetData(DataFormats.Text).ToString();
                        Regex reg = new Regex(@"http://huaban.com/boards/(.*?)/?qq-pf-to=pcqq.c2c(.*?)");
                        if(reg.IsMatch(ul))
                        {
                            if(!UrlList.Contains(ul))
                            {
                                DownLoadForm down = new DownLoadForm();
                                HttpWebRequest request = HttpWebRequest.Create(ul) as HttpWebRequest;
                                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                                StreamReader sr = new StreamReader(response.GetResponseStream());
                                string str = sr.ReadToEnd();
                                sr.Close();
                                response.Close();
                                Regex regexTitle = new Regex(@"<title>(?<title>[\s\S]*?)收集");
                                down.Title = regexTitle.Match(str).Groups["title"].Value; ;
                                if(true == down.ShowDialog())
                                {

                                }
                                down.Closed += (ss, ee) => 
                                down.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                                Dispatcher.Run();
                                Dispatcher.C
                                UrlList.Add(ul);
                            }
                            
                            
                            // DownFun(ul);
                        }
                    }
                }
            }));
            apps.SetApartmentState(ApartmentState.STA);
            apps.Start();
        }

        private void DownFun(string url)
        {
            var watch = new Stopwatch();
            watch.Start();

            var argUrl = "";
            if (Config.Debug)
            {
                argUrl = Config.ExampleUrl;
            }
            else if (url.Length > 0)
            {
                argUrl = url;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("    请指定图片所在网页(如{0})", Config.ExampleUrl);
                Console.WriteLine("    后可跟线程数量,如({0} {1})", Config.ExampleUrl, Config.ThreadCount);
                Console.WriteLine();
                WriteWithColor(() =>
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("  花瓣网-画板下载-命令行工具 by Magicdawn 2014-7-23");
                });
                return;
            }

            /*
             * 1.请求地址,
             */
            var html = Magicdawn.HttpHelper.Request(argUrl);
            var title = HuabanUtil.FindTitle(html);
            var username = HuabanUtil.FindUsername(title);
            var count = int.Parse(HuabanUtil.FindCount(title));
            var width = count.ToString().Length;
            var errorPath = "{0}/{1}/{2}".format(Config.ImageDir, title, Config.ErrorLog);
            Queue<Tuple<string, string, string>> pins = new Queue<Tuple<string, string, string>>();


            /*
             * title路径合法 ?
             */
            if (title.ContainOneOf("/ \\ : * ? \" < > |"))
            {
                Console.WriteLine("title中有不合法内容,不能做文件夹名。");
                Console.Write("请手动指定 : ");
                title = Console.ReadLine();
            }


            /*
             * 文件夹是否存在
             */
            Console.WriteLine("系列为 : {0}", title);
            Console.WriteLine("画板共 {0} 张图 , 作者为 : {1}", count, username);

            if (!Directory.Exists(Config.ImageDir))
                Directory.CreateDirectory(Config.ImageDir);
            if (Directory.Exists(Config.ImageDir + "/" + title))
            {
                Console.WriteLine();
                Console.Write("你好像下载过了...要重新下?(y/n) : ");
                watch.Stop();
                if (Console.ReadLine() != "y")
                {
                    return;//退出
                }
                else
                {
                    //接着下载
                    File.Delete(errorPath);
                    watch.Start();
                }
            }
            else
            {
                Directory.CreateDirectory(Config.ImageDir + "/" + title);
            }

            /*
             * 添加当前页
             */
            var page_pins = HuabanUtil.FindPins(html);
            foreach (var p in page_pins)
            {
                pins.Enqueue(p);
            }

            /*
             * 访问后续页
             */
            var pageNum = count / 100 + 1;
            foreach (var i in Enumerable.Range(0, pageNum))
            {
                var maxId = pins.Last().Item1; //id,src,ext
                var urls = "{0}?max={1}&limit=100".format(argUrl, maxId);

                html = Magicdawn.HttpHelper.Request(urls);
                page_pins = HuabanUtil.FindPins(html);
                foreach (var p in page_pins)
                {
                    pins.Enqueue(p);
                }
            }


            /*
             * 开始下载
             */
            var index = 1; //要处理的索引
            if (url.Length > 1)
            {
                // url 线程数量
                Config.ThreadCount = 5;
            }
            var threads = new Thread[Config.ThreadCount];

            for (int i = 0; i < Config.ThreadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    Tuple<string, string, string> p;
                    var client = new System.Net.WebClient();
                    string curIndex; //当前是第几张图
                    while (pins.Count > 0)
                    {
                        lock (pins)
                        {
                            p = pins.Dequeue();
                            curIndex = index.ToString().PadLeft(width, '0');
                            index++;
                        }

                        /*
                         * 有pin = (id,src,type)了,找url path ext
                         * 下载
                         */
                        var src = p.Item2; //()
                        var ext = p.Item3;
                        var path = "{0}/{1}/{2}.{3}".format(Config.ImageDir, title, curIndex, ext);

                        Console.WriteLine("正在下载第{0}张图 : {1}", curIndex, src);
                        //Console.WriteLine(curIndex + "@" + Thread.CurrentThread.ManagedThreadId);
                        if (!HuabanUtil.Download(client, src, path))
                        {
                            Console.WriteLine("第{0}张图下载失败!", curIndex);
                            File.AppendAllText(errorPath,
                                //2014-7-23 20:13:38 第001张 http://xxx
                                "{0} 第{1}张 {2}".format(
                                    DateTime.Now.ToStringX(), //时间
                                    curIndex, //第几张
                                    src
                                )
                            );
                        }
                    }
                }) { IsBackground = true };
                threads[i].Start();
            }

            while (CheckAnyRunning(threads))
            {
                Thread.Sleep(1000);
            }
            //等待其他线程作业
            Console.WriteLine("下载完成了...耗时 {0}分{1}秒", watch.Elapsed.Minutes, watch.Elapsed.Seconds);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
