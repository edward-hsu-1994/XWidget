using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XWidget.Web.SSO {
    public static class Providers {
        public const string Facebook = "Facebook";

        public static string[] GetAllProviders() {
            return MethodBase.GetCurrentMethod().DeclaringType.GetFields().Select(x => {
                if (x.GetCustomAttribute<ObsoleteAttribute>() != null) {
                    return null;
                }
                return (string)x.GetValue(null);
            }).Where(x => x != null).ToArray();
        }

        /// <summary>
        /// 修正大小寫
        /// </summary>
        /// <param name="input">輸入字串</param>
        /// <param name="allowNull">忽略空值問題</param>
        /// <returns>修正後的列舉值</returns>
        public static string FixCase(string input, bool allowNull = false) {
            try {
                return GetAllProviders().First(x => x.Equals(input, StringComparison.CurrentCultureIgnoreCase));
            } catch {
                if (allowNull && input == null) return null;
                throw new ArgumentException("Invaild type");
            }
        }

        public static IProvider GetProviderInstance(string provider) {
            provider = FixCase(provider);

            return null;
        }
    }
}
