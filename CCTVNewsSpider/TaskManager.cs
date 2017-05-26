using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CCTVNewsSpider
{
    static class TaskManager
    {
        /// <summary>
        /// 分配解析任务
        /// </summary>
        /// <returns>所有新闻联播文字稿</returns>
        public static List<CCTVNews> AssignAndPerformTask()
        {
            var newsPages = new BlockingCollection<Uri>();
            var result = new ConcurrentBag<CCTVNews>();

            // 解析目录页
            var producer = new Task(async () =>
            {
                var catalogPageUri = new Uri("http://mrxwlb.com/category/mrxwlb-text/");
                Tuple<Uri, List<Uri>> tuple;
                int count = 0;
                while (catalogPageUri != null)
                {
                    tuple = await PageParser.ParseCatalogPage(catalogPageUri);
                    tuple.Item2.ForEach(x => newsPages.Add(x));
                    catalogPageUri = tuple.Item1;
                    Console.WriteLine($"{++count} CatalogPage done!");
                }
                newsPages.CompleteAdding();
            });


            // 解析新闻页面
            var customer = new Task(() =>
            {
                while (!newsPages.IsCompleted)
                {
                    try
                    {
                        PageParser.ParseNewsPage(newsPages.Take(), result).Wait();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });

            producer.Start();
            customer.Start();
            Task.WaitAll(producer, customer);

            return result.ToList();
        }
    }
}
