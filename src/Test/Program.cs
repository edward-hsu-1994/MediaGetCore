using MediaGetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test {
    public class Program {
        public static void Main(string[] args) {
            ExtractorAdapter adp = new ExtractorAdapter();
            adp.IncludeAllExtractors();

            //Test(adp, "https://www.youtube.com/watch?v=b1P-SQASlMs");
            //Test(adp, "http://vlog.xuite.net/play/N2gzU21jLTE2MDQ4MDQuZmx2");
            //Test(adp, "http://vlog.xuite.net/play/QkIyTVNtLTg3MTg0Mi5mbHY=");
            Test(adp, "http://www.dailymotion.com/video/x3lada6");
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
