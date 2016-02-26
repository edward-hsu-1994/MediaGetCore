using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaGetCore.Factories {
    public static class NodeJsFactory{
        public delegate string Func(params object[] Args);

        /// <summary>
        /// 取得目前環境是否支援NodeJs
        /// </summary>
        private static Lazy<bool> LazyIsSupport = new Lazy<bool>(() => {
            ProcessStartInfo nodeJsVersion = new ProcessStartInfo("node","-v");
            nodeJsVersion.UseShellExecute = false;
            nodeJsVersion.RedirectStandardOutput = true;
            nodeJsVersion.StandardOutputEncoding = Encoding.UTF8;
            nodeJsVersion.RedirectStandardError = true;
            nodeJsVersion.StandardErrorEncoding = Encoding.UTF8;
            nodeJsVersion.CreateNoWindow = true;
            
            try {
                Process command = Process.Start(nodeJsVersion);
                return command.StandardOutput.ReadToEnd()?.Length > 0;
            } catch {
                return false;
            }
        });
        public static bool IsSupport => LazyIsSupport.Value;
        
        public static Func<string, string> CustomScriptHandler { get; set; } = null;

        public static string RunScript(string Script,string OutputFunction = "console.log") {
            Guid tempFileName = Guid.NewGuid();
            var OKScript = Script.Replace(OutputFunction, "console.log");
            if(CustomScriptHandler != null) {
                return CustomScriptHandler.Invoke(OKScript);
            }
            string fullTempFileName = $"/tmp/{tempFileName}.js";
            using (StreamWriter textWriter = new StreamWriter(File.Create(fullTempFileName))) {
                textWriter.Write(OKScript);
            }
            
            ProcessStartInfo script = new ProcessStartInfo("node", fullTempFileName);
            script.UseShellExecute = false;
            script.RedirectStandardOutput = true;
            script.StandardOutputEncoding = Encoding.UTF8;
            script.RedirectStandardError = true;
            script.StandardErrorEncoding = Encoding.UTF8;

            Process command = Process.Start(script);
            string output = command.StandardOutput.ReadToEnd().Replace("\r", "");
            string errorOutput = command.StandardError.ReadToEnd().Replace("\r", "");

            File.Delete(fullTempFileName);
            if (output?.Length == 0) {
                throw new Exception(errorOutput);
            } else {
                return output.Substring(0, output.LastIndexOf("\n"));
            }   
        }

        public static Func EvalFunc(string Script) {
            return (Args) => {
                return NodeJsFactory.RunScript($"console.log(({Script})({string.Join(",",from t in Args select (t is string?$"\"{t}\"":t ))}))");
            };
        }
    }
}
