using MediaGetCore.Extractors;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using EzCoreKit.Extensions;
using System.Net.Http;
using System.Net;

namespace MediaGetCore.Test {
    public class DailymotionExtractorTest : ExtractorTestBase<DailymotionExtractor> {
        [Theory(DisplayName = "Extractors.Dailymotion")]
        [InlineData("http://www.dailymotion.com/video/x5nybzp")]
        [InlineData("http://www.dailymotion.com/video/x666ygp")]
        [InlineData("http://www.dailymotion.com/video/x659a37")]
        [InlineData("http://www.dailymotion.com/video/x61ki70")]
        [InlineData("http://www.dailymotion.com/video/x5y3td9")]
        [InlineData("http://www.dailymotion.com/video/x5mq6pf")]
        [InlineData("http://www.dailymotion.com/video/x5nydoh")]
        [InlineData("http://www.dailymotion.com/video/x5nyecz")]
        public override async Task GetMediaInfosAsync(string url) {
            await base.GetMediaInfosAsync(url);
        }
    }
}
