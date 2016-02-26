using MediaGetCore;
using MediaGetCore.Extractors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsole{
    public class Program{
        public static void Main(string[] args){
            //執行剖析的時候才將型別實體化，防止同時調用同一配接器的參考問題
            ExtractorAdapter adp = new ExtractorAdapter(typeof(YoutubeExtractor),typeof(XuiteExtractor));
            adp.OnProcess += OnProcess;//輔助用的事件
            adp.OnCompleted += OnCompleted;//剖析完成的結果可使用事件傳遞或方法返回
            var k = adp.GetMediaInfos("https://www.youtube.com/watch?v=rmOJPXx83DA");
            //var result = adp.GetMediaInfos("http://vlog.xuite.net/play/T3NPSm9lLTIxNjI0MzgxLmZsdg==");
        }

        private static void OnProcess(IExtractor sender, double percent) {
            Console.WriteLine($"剖析進度:{percent}");
        }
        private static void OnCompleted(IExtractor sender, MediaInfo[] result) {
            Console.WriteLine($"剖析完成!");
        }
    }
}
