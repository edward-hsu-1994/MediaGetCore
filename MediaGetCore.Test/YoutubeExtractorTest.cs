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
    [Collection("Youtube")]
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
            var mediaInfos = await Instance.GetMediaInfosAsync(url);
            Assert.NotEmpty(mediaInfos);

            Random rand = new Random((int)DateTime.Now.Ticks);
            var selectedMediaInfo = mediaInfos[rand.Next(0, mediaInfos.Length)];

            Assert.True(TestUrlStatusCodeOK(selectedMediaInfo.RealUrl));

            // https://stackoverflow.com/questions/924679/c-sharp-how-can-i-check-if-a-url-exists-is-valid
            HttpWebRequest request = WebRequest.Create(selectedMediaInfo.RealUrl) as HttpWebRequest;
            request.Method = "HEAD";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            Assert.True(response.StatusCode == HttpStatusCode.OK);

            response.Close();
        }
    }
}
