using MediaGetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNugetText {
    public class Program {
        public static void Main(string[] args) {
            ExtractorAdapter adp = new ExtractorAdapter();
            adp.IncludeAllExtractors();

            Test(adp, "https://www.youtube.com/watch?v=b1P-SQASlMs");
            Test(adp, "http://vlog.xuite.net/play/N2gzU21jLTE2MDQ4MDQuZmx2");
            Test(adp, "http://vlog.xuite.net/play/QkIyTVNtLTg3MTg0Mi5mbHY=");
            Test(adp, "http://www.dailymotion.com/video/x2zhhw1_%E6%A2%81%E9%9D%9C%E8%8C%B9-%E5%8F%AF%E6%A8%82%E6%88%92%E6%8C%87_music");
            Console.ReadKey();
        }

        private static void Test(ExtractorBase adp, string Url) {
            var infos = adp.GetMediaInfos(Url);
            foreach (var info in infos) {
                Console.WriteLine(
                    $"Title:\t{info.Name}\n" +
                    $"RealUrl:\t{info.RealUrl}\n" +
                    new string('=', 20)
                );
            }
        }
    }
}
