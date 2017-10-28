using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediaGetCore.Attributes;

namespace MediaGetCore {
    /// <summary>
    /// 媒體剖析器基礎類別
    /// </summary>
    public abstract class ExtractorBase : IExtractor {
        /// <inheritdoc />
        public abstract Task<MediaInfo[]> GetMediaInfoAsync(string url);

        /// <inheritdoc />
        public virtual bool IsMatch(string url) {
            var urlMatchs = this.GetType().GetCustomAttributes<ExtractorUrlMatchAttribute>();
            return urlMatchs.Any(x => x.IsMatch(url));
        }

        #region 多載

        /// <inheritdoc />
        public Task<MediaInfo[]> GetMediaInfoAsync(Uri url) => this.GetMediaInfoAsync(url.OriginalString);

        /// <inheritdoc />
        public bool IsMatch(Uri url) => this.IsMatch(url.OriginalString);

        #endregion
    }
}
