using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVNewsSpider
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            var before = DateTime.Now;

            var newsList = TaskManager.AssignAndPerformTask();

            var dataFolder = new DirectoryInfo(@"..\..\cctc_news");
            if (!dataFolder.Exists) dataFolder.Create();
            foreach (var news in newsList)
            {
                var file = File.OpenWrite(Path.Combine(dataFolder.FullName, news.Date.ToShortDateString().Replace('/', '_') + ".txt"));
                using (var sw = new StreamWriter(file))
                {
                    sw.WriteLine(news.Date.ToLongDateString());
                    sw.WriteLine(news.Content);
                }
            }

            using (var sw = new StreamWriter(Path.Combine(dataFolder.FullName, "ErrorUri.txt")))
            {
                ParserTools.ErrorUris.ForEach(x => sw.WriteLine(x.OriginalString));
            }

            var after = DateTime.Now;
            Console.WriteLine(after - before);
            // 5min:53s
        }
    }
}
