using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XWidget.Utilities {
    /// <summary>
    /// 針對<see cref="Type"/>與<see cref="TypeInfo"/>常用方法
    /// </summary>
    public static class TypeUtility {
        /// <summary>
        /// 取得指定命名空間內所有類型陣列
        /// </summary>
        /// <param name="ns">指定命名空間</param>
        /// <returns>指定命名空間內所有類型陣列</returns>
        public static Type[] GetNamespaceTypes(string ns) {
            List<Type> result = new List<Type>();
            foreach (var assembly in Assembly
                .GetEntryAssembly()
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Concat(new Assembly[] { Assembly.GetEntryAssembly() })) {
                Type[] types = null;
                try {
                    types = assembly.GetTypes();

                } catch (ReflectionTypeLoadException e) {
                    types = e.Types;
                }
                result.AddRange(types);
            }

            return result.Where(x => x.Namespace == ns).ToArray();
        }

        /// <summary>
        /// 取得指定Assembly中目標命名空間內所有類型陣列
        /// </summary>
        /// <param name="assembly">指定Assembly</param>
        /// <param name="ns">指定命名空間</param>
        /// <returns>指定命名空間內所有類型陣列</returns>
        public static Type[] GetNamespaceTypes(Assembly assembly, string ns) {
            return (from t in assembly.GetTypes()
                    where t.Namespace == ns
                    select t).ToArray();
        }

        /// <summary>
        /// 自字串轉換為指定型別
        /// </summary>
        /// <typeparam name="T">目標型別</typeparam>
        /// <param name="s">字串</param>
        /// <returns>剖析結果</returns>
        public static T Parse<T>(string s) where T : struct {
            return (T)typeof(T)
                .GetMethods()
                .First(x => x.Name == "Parse" && x.GetParameters().Length == 1)
                .Invoke(null, new object[] { s });
        }

        /// <summary>
        /// 嘗試自字串轉換為指定型別
        /// </summary>
        /// <typeparam name="T">目標型別</typeparam>
        /// <param name="s">字串</param>
        /// <param name="result">剖析結果</param>
        /// <returns>是否剖析成功</returns>
        public static bool TryParse<T>(string s, out T result) where T : struct {
            result = default(T);

            var args = new object[] { s, result };

            var result_ = (bool)typeof(T)
                .GetMethods()
                .First(x => x.Name == "TryParse" && x.GetParameters().Length == 2)
                .Invoke(
                null,
                args
            );

            result = (T)args[1];

            return result_;
        }
    }
}
