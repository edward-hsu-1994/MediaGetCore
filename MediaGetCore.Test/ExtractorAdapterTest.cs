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
    public class ExtractorAdapterTest : ExtractorTestBase<ExtractorAdapter> {
        [Theory(DisplayName = "ExtractorAdapter")]
        [InlineData("https://www.youtube.com/watch?v=bu7nU9Mhpyo")]
        [InlineData("https://vlog.xuite.net/play/U2ZBeks5LTMxMDg3MTY5LmZsdg==")]
        [InlineData("http://www.dailymotion.com/video/x4zsx6z")]
        [InlineData("https://www.youtube.com/watch?v=_VxLOj3TB5k")]
        [InlineData("https://www.facebook.com/chuchushoeTW/videos/1687449004612611/")]
        [InlineData("https://www.facebook.com/chuchushoeTW/videos/1680508941973284/")]
        [InlineData("https://vlog.xuite.net/play/NWZ6cGFkLTg1MjUxMDQuZmx2")]
        [InlineData("http://www.dailymotion.com/video/x5iiw1g")]
        [InlineData("https://www.youtube.com/watch?v=jNQXAC9IVRw")]
        [InlineData("https://www.youtube.com/watch?v=MuidrWY30Xk")]
        public override async Task GetMediaInfosAsync(string url) {
            ExtractorAdapter adapter = this.Instance as ExtractorAdapter;
            adapter.AddDefaultExtractors();
            await base.GetMediaInfosAsync(url);
        }
    }
}
