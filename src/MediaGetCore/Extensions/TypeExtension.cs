using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaGetCore.Extensions {
    public static class TypeExtension {
        /// <summary>
        /// 取得型別的繼承鏈
        /// </summary>
        /// <param name="type">起始型別</param>
        /// <returns>繼承鏈集合</returns>
        public static TypeInfo[] GetBaseTypes(this TypeInfo type) {
            List<TypeInfo> result = new List<TypeInfo>();
            result.Add(type);
            if (type != typeof(object).GetTypeInfo()) {
                result.AddRange(type.BaseType.GetTypeInfo().GetBaseTypes());
            }
            return result.ToArray();
        }
    }
}
