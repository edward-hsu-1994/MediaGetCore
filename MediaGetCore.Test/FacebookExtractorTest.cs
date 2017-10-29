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
    public class FacebookExtractorTest : ExtractorTestBase<FacebookExtractor> {
        [Theory(DisplayName = "Extractors.Facebook")]
        [InlineData("https://www.facebook.com/yv.dimension/videos/1769042039979944/")]
        [InlineData("https://www.facebook.com/chuchushoeTW/videos/1687449004612611/")]
        [InlineData("https://www.facebook.com/chuchushoeTW/videos/1680508941973284/")]
        [InlineData("https://www.facebook.com/dmguosub/videos/1945779748998420/")]
        public override async Task GetMediaInfosAsync(string url) {
            await base.GetMediaInfosAsync(url);
        }
    }
}
