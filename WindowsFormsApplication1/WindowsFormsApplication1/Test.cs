using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class Test
    {
        private string _sitename;

        public string Sitename
        {
            get { return _sitename; }
            set { _sitename = value; }
        }
        private string _siteurl;

        public string Siteurl
        {
            get { return _siteurl; }
            set { _siteurl = value; }
        }
        private string _keyword;

        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }
        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
