using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace XWidget.Reflection {
    /// <summary>
    /// <see cref="ExpandoObject"/>幫助類別
    /// </summary>
    public static class ExpandoObjectUtility {
        /// <summary>
        /// 將目標實例轉換為<see cref="ExpandoObject"/>實例(僅轉換屬性與欄位)
        /// </summary>
        /// <param name="obj">目標實例</param>
        /// <param name="publicOnly">是否只轉換Public屬性或欄位</param>
        /// <returns><see cref="ExpandoObject"/>實例</returns>
        public static ExpandoObject ConvertToExpando(object obj, bool publicOnly = true) {
            ExpandoObject result = new ExpandoObject();
            IDictionary<string, object> dict = result;
            BindingFlags flag =
                BindingFlags.Instance |
                BindingFlags.Public;
            if (!publicOnly) flag |= BindingFlags.NonPublic;
            var allMembers = obj.GetType().GetMembers(flag);

            foreach (var member in allMembers) {
                if (member is FieldInfo) {
                    dict.Add(member.Name, ((FieldInfo)member).GetValue(obj));
                } else if (member is PropertyInfo &&
                    ((PropertyInfo)member).GetIndexParameters().Length == 0) {
                    dict.Add(member.Name, ((PropertyInfo)member).GetValue(obj));
                }
            }
            return result;
        }
    }
}
