using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EzCoreKit.Extensions;
using EzCoreKit.Reflection;
using MediaGetCore.Exceptions;

namespace MediaGetCore {
    /// <summary>
    /// 媒體剖析器配接器，用以在<see cref="IExtractor"/>集合中自動選取合適的<see cref="IExtractor"/>實例
    /// </summary>
    public sealed class ExtractorAdapter : IExtractor {
        /// <summary>
        /// 所有媒體剖析器實例
        /// </summary>
        private List<IExtractor> extractorInstances = new List<IExtractor>();

        /// <summary>
        /// 取得所有媒體剖析器類型
        /// </summary>
        public IReadOnlyList<Type> ExtractorTypes => extractorInstances.Select(x => x.GetType()).ToList().AsReadOnly();

        /// <summary>
        /// 加入MediaGetCore.Extractors內所有剖析器
        /// </summary>
        public void AddDefaultExtractors() {
            AddAllExtractorsFromNamespace("MediaGetCore.Extractors");
        }

        /// <summary>
        /// 自指定namespace中引入所有<see cref="IExtractor"/>類型
        /// </summary>
        /// <param name="ns">namespace</param>
        public void AddAllExtractorsFromNamespace(string ns) {
            var types =
                TypeHelper.GetNamespaceTypes(ns).Where(x => x.GetInterfaces().Contains(typeof(IExtractor)));

            foreach (var type in types) {
                extractorInstances.Add((IExtractor)Activator.CreateInstance(type));
            }
        }

        /// <summary>
        /// 加入媒體剖析器類型
        /// </summary>
        /// <typeparam name="T">剖析器類型</typeparam>
        public void AddExtractor<T>() where T : IExtractor, new() {
            if (ContainsType<T>()) {
                throw new InvalidOperationException("已經加入過指定類型");
            }
            extractorInstances.Add(new T());
        }

        /// <summary>
        /// 刪除指定類型的媒體剖析器
        /// </summary>
        /// <typeparam name="T">剖析器類型</typeparam>
        public void RemoveExtractor<T>() where T : IExtractor, new() {
            if (!ContainsType<T>()) {
                throw new InvalidOperationException("找不到指定類型");
            }
            extractorInstances = extractorInstances.Where(x => !(x is T)).ToList();
        }

        /// <summary>
        /// 檢查目前配接器是否已經加入過指定的剖析器類型
        /// </summary>
        /// <typeparam name="T">剖析器類型</typeparam>
        /// <returns>是否已經加入過指定剖析器類型</returns>
        public bool ContainsType<T>() where T : IExtractor, new() {
            return extractorInstances.Any(x => x is T);
        }

        /// <summary>
        /// 嘗試加入媒體剖析器類型
        /// </summary>
        /// <typeparam name="T">剖析器類型</typeparam>
        /// <returns>是否成功加入</returns>
        public bool TryAddExtractor<T>() where T : IExtractor, new() {
            if (ContainsType<T>()) return false;
            AddExtractor<T>();
            return true;
        }

        /// <summary>
        /// 移除指定類型的媒體剖析器
        /// </summary>
        /// <typeparam name="T">剖析器類型</typeparam>
        /// <returns>是否成功移除</returns>
        public bool TryRemoveExtractor<T>() where T : IExtractor, new() {
            if (!ContainsType<T>()) return false;
            RemoveExtractor<T>();
            return true;
        }

        /// <inheritdoc />
        public async Task<MediaInfo[]> GetMediaInfosAsync(string url) {
            if (!IsMatch(url)) throw new UrlNotSupportedException();
            var extractor = extractorInstances.FirstOrDefault(x => x.IsMatch(url));

            return await extractor.GetMediaInfosAsync(url);
        }

        /// <inheritdoc />
        public bool IsMatch(string url) => extractorInstances.Any(x => x.IsMatch(url));

        #region 多載
        /// <inheritdoc />
        public Task<MediaInfo[]> GetMediaInfosAsync(Uri url) => GetMediaInfosAsync(url.OriginalString);

        /// <inheritdoc />
        public bool IsMatch(Uri url) => IsMatch(url.OriginalString);
        #endregion
    }
}
