using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaGetCore{
    public class JObjectBinder<T>{
        public Dictionary<MethodCallExpression, MemberInfo> Mapping { get; set; }= new Dictionary<MethodCallExpression, MemberInfo>();

        public void Bind(Expression<Func<JObject, object>> From, Expression<Func<T, object>> To) {
            MethodCallExpression fromBody = (MethodCallExpression)From.Body;
            MemberExpression toBody = (MemberExpression)(((UnaryExpression)To.Body).Operand);
            Mapping[fromBody] = toBody.Member; 
        }

        public T ToObject(JObject Obj) {
            T result = Activator.CreateInstance<T>();
            foreach(var item in Mapping) {
                JValue value = (JValue)item.Key.Method.Invoke(Obj, item.Key.Arguments.Select(item2=>((ConstantExpression)item2).Value).ToArray());
                if(item.Value is PropertyInfo) {
                    var c = ((PropertyInfo)item.Value);
                    c.SetValue(result, Convert.ChangeType(value.Value, c.PropertyType));
                } else if(item.Value is FieldInfo) {
                    var c = ((FieldInfo)item.Value);
                    c.SetValue(result,Convert.ChangeType(value.Value,c.FieldType));
                }
            }
            return result;
        }
    }
}
