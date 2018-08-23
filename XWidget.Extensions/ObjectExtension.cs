using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace XWidget.Extensions {
    /// <summary>
    /// 針對<see cref="Object"/>的擴充方法
    /// </summary>
    public static class ObjectExtension {
        /// <summary>
        /// 取得目前實例指定私有欄位值
        /// </summary>
        /// <typeparam name="T">值類別</typeparam>
        /// <param name="obj">目前實例</param>
        /// <param name="fieldName">欄位名稱</param>
        /// <returns>欄位值</returns>
        public static T GetPrivateFieldValue<T>(this object obj, string fieldName) {
            TypeInfo temp = obj.GetType().GetTypeInfo();
            while (temp != typeof(object).GetTypeInfo()) {
                var fieldInfo = obj.GetType().GetTypeInfo().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null) continue;
                return (T)fieldInfo.GetValue(obj);
            }
            return default(T);
        }

        /// <summary>
        /// 將父類型物件轉換為子類型物件
        /// </summary>
        /// <typeparam name="TBase">父類型</typeparam>
        /// <typeparam name="TChild">子類型</typeparam>
        /// <param name="obj">轉換物件實例</param>
        /// <returns>轉換結果實例</returns>
        public static TChild ToChildType<TBase, TChild>(this TBase obj) where TChild : TBase {
            return ToChildType(obj, () => Activator.CreateInstance<TChild>());
        }

        /// <summary>
        /// 將父類型物件轉換為子類型物件
        /// </summary>
        /// <typeparam name="TBase">父類型</typeparam>
        /// <typeparam name="TChild">子類型</typeparam>
        /// <param name="instanceCreate">實例建構方法</param>
        /// <param name="obj">轉換物件實例</param>
        /// <returns>轉換結果實例</returns>
        public static TChild ToChildType<TBase, TChild>(this TBase obj, Func<TChild> instanceCreate) where TChild : TBase {
            TChild instance = instanceCreate();

            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                prop.SetValue(instance, prop.GetValue(obj));
            }

            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                field.SetValue(instance, field.GetValue(obj));
            }

            return instance;
        }

        /// <summary>
        /// 針對物件處理
        /// </summary>
        /// <typeparam name="T">物件類型</typeparam>
        /// <param name="obj">物件實力</param>
        /// <param name="action">物件實例</param>
        /// <returns>物件實例</returns>
        public static T Process<T>(this T obj, Action<T> action)
            where T : class {
            action(obj);

            return obj;
        }

        /// <summary>
        /// 將物件轉換為<see cref="byte[]"/>
        /// </summary>
        /// <param name="obj">物件實例</param>
        /// <returns>Binary Data</returns>
        public static byte[] Serialize(this object obj) {
            BinaryFormatter sf = new BinaryFormatter();
            return sf.Serialize(obj);
        }
    }
}
