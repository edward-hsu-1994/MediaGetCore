using ChakraCore.NET;

namespace MediaGetCore.Helpers {
    /// <summary>
    /// Javascript執行幫助類別
    /// </summary>
    public static class JavascriptHelper {
        /// <summary>
        /// Chakra執行階段
        /// </summary>
        private static ChakraRuntime runtimeInstance;

        /// <summary>
        /// Chakra內容
        /// </summary>
        private static ChakraContext contextInstance;

        /// <summary>
        /// 初始化Javascript幫助類別
        /// </summary>
        static JavascriptHelper() {
            runtimeInstance = ChakraRuntime.Create();
            contextInstance = runtimeInstance.CreateContext(true);
        }

        /// <summary>
        /// 執行輸入的Javascript腳本
        /// </summary>
        /// <param name="script">Javascript腳本</param>
        /// <returns>Javascript執行結果</returns>
        public static string Run(string script) => contextInstance.RunScript(script);
    }
}
