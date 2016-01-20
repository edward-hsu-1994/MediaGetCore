using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGetCore.Extensions{
    public static class UriBuilderExtension{
        public static Dictionary<string,string> GetParamDictionary(this UriBuilder Obj) {
            return Obj.Query?.Substring(1)
                .Split(new char[] { '&' },StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Split(new char[] { '=' },2,StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(
                    item=>item[0],
                    item=>Uri.UnescapeDataString(item[1])
                );
        }

        public static void SetParamDictionary(this UriBuilder Obj,Dictionary<string,string> Data) {
            if (Data.Count == 0) return;
            Obj.Query = string.Join("&",Data.Select(item => $"{item.Key}={Uri.EscapeDataString(item.Value)}"));
        }


        public static string GetQueryParam(this UriBuilder Obj, string Key) {
            Dictionary<string,string> Data = Obj.GetParamDictionary();
            if (Data.ContainsKey(Key)) return Data[Key];
            return null;
        }

        public static void RemoveQueryParam(this UriBuilder Obj, string Key) {
            Dictionary<string, string> Data = Obj.GetParamDictionary();
            Data.Remove(Key);
            Obj.SetParamDictionary(Data);
        }

        public static void SetQueryParam(this UriBuilder Obj, string Key,string Value) {
            Dictionary<string, string> Data = Obj.GetParamDictionary();
            Data[Key] = Value;
            Obj.SetParamDictionary(Data);
        }


    }
}
