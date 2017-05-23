using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVNewsSpider
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            PageParser.ParsePage(new Uri(@"http://mrxwlb.com/2017%E5%B9%B45%E6%9C%8823%E6%97%A5%E6%96%B0%E9%97%BB%E8%81%94%E6%92%AD%E6%96%87%E5%AD%97%E7%89%88/")).Wait();
        }
    }
}
