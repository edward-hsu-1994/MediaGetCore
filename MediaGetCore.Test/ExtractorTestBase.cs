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

        public abstract Task GetMediaInfosAsync(string url);


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
