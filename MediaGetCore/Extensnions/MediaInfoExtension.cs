using System.IO;
using System.Threading.Tasks;
using MediaGetCore.Helpers;

namespace MediaGetCore.Extensnions {
    /// <summary>
    /// 針對<see cref="MediaInfo"/>的擴充方法
    /// </summary>
    public static class MediaInfoExtension {
        /// <summary>
        /// 下載到指定的路徑
        /// </summary>
        /// <param name="obj"><see cref="MediaInfo"/>實例</param>
        /// <param name="savePath">儲存路徑</param>
        public static async void Download(this MediaInfo obj, string savePath) {
            Stream stream = await obj.GetStreamAsync();

            FileStream file = File.Create(savePath);

            byte[] buffer = new byte[1024 * 4];
            using (BinaryWriter writer = new BinaryWriter(file)) {
                int count = -1;
                while (count != -1 && count < buffer.Length) {
                    count = stream.Read(buffer, 0, buffer.Length);
                    writer.Write(buffer, 0, count);
                }
            }
        }

        /// <summary>
        /// 取得下載串流
        /// </summary>
        /// <param name="obj"><see cref="MediaInfo"/>實例</param> 
        /// <returns>媒體串流</returns>
        public static async Task<Stream> GetStreamAsync(this MediaInfo obj) {
            return await DownloadHelper.GetDownloadStreamAsync(obj.RealUrl);
        }
    }
}
