using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Magicdawn.Http;
using System.Net;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static object Deserialize(string json, Type serializedObjectType)
        {
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(serializedObjectType);
            object result;
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                result = dataContractJsonSerializer.ReadObject(memoryStream);
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*var html = Magicdawn.HttpHelper.Request("http://huaban.com/boards/12805807/?qq-pf-to=pcqq.c2c");
            StreamWriter sw =new StreamWriter("C:/Users/Administrator/Desktop/新建文件夹/test.html");
            sw.Write(html);
            sw.Close();*/

            HttpWebRequest request = HttpWebRequest.Create("http://huaban.com/boards/12805807") as HttpWebRequest;
            WebResponse response = request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream());
            string str = sr.ReadToEnd();
            sr.Close();

            StreamWriter sw = new StreamWriter("C:/Users/Administrator/Desktop/新建文件夹/test.html");
            sw.Write(str);
            sw.Close();
        }
    }
}
