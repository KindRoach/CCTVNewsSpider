using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVNewsSpider
{
    class CCTVNews
    {
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public CCTVNews(DateTime date, string content)
        {
            Date = date;
            Content = content;
        }
    }
}
