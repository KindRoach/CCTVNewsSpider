using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Concurrent;
using System.Threading;

namespace CCTVNewsSpider
{
    static class PageParser
    {
        /// <summary>
        /// 析新闻页面,并将新闻稿添加到指定集合
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public static async Task ParseNewsPage(Uri uri, ConcurrentBag<CCTVNews> news)
        {
            try
            {
                var http = new HttpClient();
                var html = await http.GetStringAsync(uri);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var title = htmlDoc.DocumentNode.FindByClass("entry-title").First();
                var date = Convert.ToDateTime(title.InnerText.Substring(0, title.InnerText.IndexOf("新闻联播")).Trim());

                var section = htmlDoc.DocumentNode.FindByClass("entry-content").First();
                var pNodes = section.Descendants("p").Skip(2);

                var sb = new StringBuilder();
                foreach (var node in pNodes)
                {
                    sb.Append(node.InnerText.Trim() + Environment.NewLine);
                }
                sb = sb.Replace("<!--repaste.body.end-->", string.Empty)
                       .Replace("&ldquo;", "“")
                       .Replace("&rdquo;", "”")
                       .Replace("&middot;", "·");
                Console.WriteLine($"{news.Count} NewsPage done!");
                news.Add(new CCTVNews(date, sb.ToString()));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ParserTools.RecordErrorUri(uri);
            }
        }


        /// <summary>
        /// 解析所有目录页面,每个目录页面包括多个新闻页面的链接
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>下一目录页的Uri和本页所有新闻页面的Uri</returns>
        public static async Task<Tuple<Uri, List<Uri>>> ParseCatalogPage(Uri uri)
        {
            Uri nextPageUri = null;
            var uris = new List<Uri>();
            try
            {
                var http = new HttpClient();
                var html = await http.GetStringAsync(uri);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // 查找下一目录页
                var nextNodes = htmlDoc.DocumentNode.FindByClass("next page-numbers");
                if (nextNodes.Any())
                {
                    nextPageUri = new Uri(nextNodes.First().Attributes["href"].Value);
                }

                // 查找所有新闻页
                var titleNodes = htmlDoc.DocumentNode.FindByClass("entry-title");
                foreach (var title in titleNodes)
                {
                    var aNode = title.Descendants("a").First();
                    uris.Add(new Uri(aNode.Attributes["href"].Value));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ParserTools.RecordErrorUri(uri);
            }
            return new Tuple<Uri, List<Uri>>(nextPageUri, uris);
        }
    }

    static class ParserTools
    {
        /// <summary>
        /// 按class查找子Node
        /// </summary>
        /// <param name="root"></param>
        /// <param name="className"></param>
        /// <returns>所有拥有指定class的Node</returns>
        public static List<HtmlNode> FindByClass(this HtmlNode root, string className)
        {
            return root.Descendants()
                   .Where(x => x.Attributes.Contains("class") &&
                               x.Attributes["class"].Value == className)
                   .ToList();
        }

        public static List<Uri> ErrorUris { get; } = new List<Uri>();

        /// <summary>
        /// 记录连接错误的Uri
        /// </summary>
        /// <param name="uri"></param>
        public static void RecordErrorUri(Uri uri)
        {
            ErrorUris.Add(uri);
        }
    }
}
