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
    public class XuiteExtractorTest : ExtractorTestBase<XuiteExtractor> {
        [Theory]
        [InlineData("https://vlog.xuite.net/play/VnFsT3dILTYzODYxOS5mbHY=")]
        [InlineData("https://vlog.xuite.net/play/ZUFTU0ZNLTMwNzY4NTEzLmZsdg==")]
        [InlineData("https://vlog.xuite.net/play/VHlqbVJILTMxMDg3NjUxLmZsdg==")]
        [InlineData("https://vlog.xuite.net/play/RVp6enBBLTU1NDU0My5mbHY=")]
        [InlineData("https://vlog.xuite.net/play/MXJuTHhJLTc0MTA4Mi5mbHY=")]
        [InlineData("https://vlog.xuite.net/play/VUhtaDgzLTIwMzg2NzUxLmZsdg==")]
        [InlineData("https://vlog.xuite.net/play/amtvQ1NMLTMxMDg3MTg1LmZsdg==")]
        [InlineData("https://vlog.xuite.net/play/YVpUUEZaLTIxOTgzMzM5LmZsdg==")]
        public override async Task GetMediaInfosAsync(string url) {
            await base.GetMediaInfosAsync(url);
        }
    }
}
