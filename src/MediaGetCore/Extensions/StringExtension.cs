using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGetCore.Extensions{
    /// <summary>
    /// 針對String類型的擴充
    /// </summary>
    public static class StringExtension {
        /// <summary>
        /// 在現有字串內群找自<paramref name="Start"/>開始到<paramref name="End"/>結束間的子字串
        /// </summary>
        /// <param name="Target">擴充對象</param>
        /// <param name="Start">起始字串</param>
        /// <param name="End">結束字串</param>
        /// <returns>子字串</returns>
        public static string InnerString(this string Target, string Start, string End) {
            if (Target.IndexOf(Start) < 0) return null;
            Target = Target.Substring(Target.IndexOf(Start) + Start.Length);
            return Target.Substring(0, Target.IndexOf(End));
        }
    }
}
