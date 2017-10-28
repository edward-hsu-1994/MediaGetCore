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
    public class YoutubeExtractorTest : ExtractorTestBase<YoutubeExtractor> {
        [Theory]
        [InlineData("https://www.youtube.com/watch?v=O_icXlxB4jI")]
        [InlineData("https://www.youtube.com/watch?v=P32pMWRlZx8")]
        [InlineData("https://www.youtube.com/watch?v=LAPXqtxxstE")]
        [InlineData("https://www.youtube.com/watch?v=qIF8xvSA0Gw")]
        [InlineData("https://www.youtube.com/watch?v=kK1MaJ6xX-s")]
        [InlineData("https://www.youtube.com/watch?v=ORuSpJUA-2s")]
        [InlineData("https://www.youtube.com/watch?v=DkFnqo5GZGg")]
        [InlineData("https://www.youtube.com/watch?v=pK9gqgA5duA")]
        public override async Task GetMediaInfosAsync(string url) {
            await base.GetMediaInfosAsync(url);
        }
    }
}
