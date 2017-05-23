using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CCTVNewsSpider
{
    static class PageParser
    {
        public static async Task ParsePage(Uri uri)
        {
            var http = new HttpClient();
            var html = await http.GetStringAsync(uri);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var title = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"post-66\"]/header/h1").First();
            var date = Convert.ToDateTime(title.InnerText.Substring(0, title.InnerText.IndexOf("新闻联播")));

            var section = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"post-66\"]/section[1]").First();
            var pNodes = section.Descendants("p").Skip(2);

            var s = new StringBuilder();
            foreach (var node in pNodes)
            {
                s.Append(node.InnerText);
            }
            Console.WriteLine(s.ToString());
        }
    }
}
