using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaGetCore.Factories {
    public static class JsFactory{
        public delegate string Func(params object[] Args);
        public static Func<string, string> ScriptHandler { get; set; } = NodeJsScriptHandler;

        public readonly static Func<string, string> NodeJsScriptHandler = (Script) => {
            Guid tempFileName = Guid.NewGuid();

            string fullTempFileName = $"/tmp/{tempFileName}.js";
            using (StreamWriter textWriter = new StreamWriter(File.Create(fullTempFileName))) {
                textWriter.Write(Script);
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
        };
        

        public static string RunScript(string Script,string OutputFunction = "console.log") {
            var OKScript = Script.Replace(OutputFunction, "console.log");
            if(ScriptHandler != null) {
                return ScriptHandler.Invoke(OKScript);
            } else {
                return NodeJsScriptHandler(OKScript);
            }            
        }

        public static Func EvalFunc(string Script) {
            return (Args) => {
                return JsFactory.RunScript($"console.log(({Script})({string.Join(",",from t in Args select (t is string?$"\"{t}\"":t ))}))");
            };
        }
    }
}
