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
            
            var infos = adp.GetMediaInfos("https://www.youtube.com/watch?v=b1P-SQASlMs");
            foreach (var info in infos) {
                Console.WriteLine(
                    $"Title:\t{info.Name}\n" +
                    $"RealUrl:\t{info.RealUrl}\n" +
                    new string('=', 20)
                );
            }
            Console.ReadKey();
        }
    }
}
