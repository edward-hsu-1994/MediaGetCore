using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;

namespace MediaGetCore.Helpers {
    /// <summary>
    /// Javascript執行幫助類別
    /// </summary>
    public static class JavascriptHelper {
        /// <summary>
        /// Chakra引擎
        /// </summary>
        private static IJsEngine engine;

        /// <summary>
        /// 初始化Javascript幫助類別
        /// </summary>
        static JavascriptHelper() {
            engine = new ChakraCoreJsEngine(
                new ChakraCoreSettings {
                    DisableEval = true,
                    EnableExperimentalFeatures = true
                }
            );
        }

        /// <summary>
        /// 執行輸入的Javascript腳本
        /// </summary>
        /// <param name="script">Javascript腳本</param>
        /// <returns>Javascript執行結果</returns>
        public static string Run(string script) => engine.Evaluate<string>(script);
    }
}
