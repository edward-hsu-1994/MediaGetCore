using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MediaGetCore.Test {
    public abstract class ExtractorTestBase<T> where T : IExtractor, new() {
        public IExtractor Instance { get; private set; }

        public ExtractorTestBase() {
            Instance = new T();
        }

        public virtual async Task GetMediaInfosAsync(string url) {
            var mediaInfos = await Instance.GetMediaInfosAsync(url);
            Assert.NotEmpty(mediaInfos);

            Random rand = new Random((int)DateTime.Now.Ticks);
            var selectedMediaInfo = mediaInfos[rand.Next(0, mediaInfos.Length)];

            Assert.True(TestUrlStatusCodeOK(selectedMediaInfo.RealUrl));
        }


        public bool TestUrlStatusCodeOK(Uri url) {
            // https://stackoverflow.com/questions/924679/c-sharp-how-can-i-check-if-a-url-exists-is-valid
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "HEAD";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            var result = response.StatusCode == HttpStatusCode.OK;
            response.Close();

            return result;
        }
    }
}
