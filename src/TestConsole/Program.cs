using MediaGetCore;
using MediaGetCore.Extractors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace TestConsole{
    public class Program{
        public static void Main(string[] args){
            //執行剖析的時候才將型別實體化，防止同時調用同一配接器的參考問題
            ExtractorAdapter adp = new ExtractorAdapter(typeof(YoutubeExtractor),typeof(XuiteExtractor));

            var testUrls = new string[]{
                "http://vlog.xuite.net/play/ZWtGQWF6LTE1NjI1ODEyLmZsdg==",
                "https://www.youtube.com/watch?v=rmOJPXx83DA", 
                "https://www.youtube.com/watch?v=AXE39T0uHAA",
                "https://www.youtube.com/watch?v=uEcKDfMS_jY",
                "https://www.youtube.com/watch?v=F9QBbYMEPmo"
            };

            foreach(var url in testUrls) {
                var result = adp.GetMediaInfos(url);

                Console.WriteLine(result.First().RealUrl);
            }
            Console.ReadKey();
        }
        
    }
}
