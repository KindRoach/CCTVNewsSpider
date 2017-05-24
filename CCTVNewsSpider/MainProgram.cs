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
            var newsList = TaskManager.AssignAndPerformTask().OrderBy(x => x.Date);
            var dataFolder = new DirectoryInfo("cctc_news");
            if (!dataFolder.Exists) dataFolder.Create();
            foreach (var news in newsList)
            {
                File.Create(Path.Combine(dataFolder))
            }
        }
    }
}
