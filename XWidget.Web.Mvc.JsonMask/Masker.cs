using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XWidget.Web.Mvc.JsonMask {
    /// <summary>
    /// 屬性屏蔽核心
    /// </summary>
    public static class Masker {
        /// <summary>
        /// 深層複製方法，預設為Force.DeepCloner
        /// </summary>
        public static Func<object, object> DeepClone = (obj) => {
            return obj.DeepClone();
        };

        /// <summary>
        /// 取得屬性屏蔽後的結果
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="data">資料</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns>屏蔽後的資料</returns>
        public static TData Mask<TData>(
            TData data,
            string patternName) {
            // 引動內部屏蔽方法，並深層複製原始資料，中斷參考關係
            return InternalMask(data.GetType(), (TData)DeepClone(data), null, patternName, new List<object>());
        }

        /// <summary>
        /// 取得屬性屏蔽後的結果
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="data">資料</param>
        /// <param name="controller">控制實例</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns>屏蔽後的資料</returns>
        public static TData Mask<TData, TController>(
            TData data,
            TController controller,
            string patternName = null)
            where TController : Controller {
            // 引動內部屏蔽方法，並深層複製原始資料，中斷參考關係
            return InternalMask(data.GetType(), (TData)DeepClone(data), controller, patternName, new List<object>());
        }

        internal static TData InternalMask<TData>(
            Type declaringType,
            TData data,
            object controller,
            string patternName,
            List<object> refList) {

            // 檢查是否發生參考循環
            if (refList.Contains(data)) {
                // 發生參考循環則直接返回
                return data;
            }
            // 加入參考列表
            refList.Add(data);

            #region 可列舉型別處理
            if (data is IEnumerable enumData) {
                foreach (var ele in enumData) {
                    InternalMask(declaringType, ele, controller, patternName, refList)
                        .DeepCloneTo(ele);
                }
                return data;
            }
            #endregion

            var type = data.GetType();

            #region 排除命名空間為System的類型
            if (type.Namespace == nameof(System)) {
                return data;
            }
            #endregion

            #region 取得該類型中非靜態的所有屬性
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                // 取得該屬性的JsonPropertyMaskAttribute集合，如果未設定則應該為空集合
                var attrs = property.GetCustomAttributes<JsonPropertyMaskAttribute>();

                // 在JsonMask設定集合中尋找是否有符合項目
                if (attrs.Any(x => x.IsMatch(controller as Controller, declaringType, patternName))) {
                    if (property.CanWrite) {
                        // 找到符合項目則設定屏蔽
                        property.SetValue(data, null);
                    }
                } else {
                    // 該屬性找不到屏蔽設定，檢查該屬性的屬性類型是否有屏蔽選項
                    var propertyType = property.PropertyType;

                    // 重設屬性值，檢驗該屬性可寫入並且類型不是System命名空間內的
                    if (property.CanWrite && propertyType.Namespace != nameof(System)) {
                        var value = property.GetValue(data);
                        if (value == null) continue;

                        // 屏蔽子項目並重設屬性值
                        property.SetValue(
                            data,
                            InternalMask(
                                propertyType,
                                value,
                                controller,
                                patternName,
                                refList
                            )
                        );
                    }
                }
            }
            #endregion

            #region 取得該欄位中非靜態的所有屬性
            foreach (var filed in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                // 取得該欄位的JsonPropertyMaskAttribute集合，如果未設定則應該為空集合
                var attrs = filed.GetCustomAttributes<JsonPropertyMaskAttribute>();

                // 在JsonMask設定集合中尋找是否有符合項目
                if (attrs.Any(x => x.IsMatch(controller as Controller, declaringType, patternName))) {
                    // 找到符合項目則設定屏蔽
                    filed.SetValue(data, null);
                } else {
                    // 該屬性找不到屏蔽設定，檢查該欄位的屬性類型是否有屏蔽選項
                    var filedType = filed.FieldType;

                    // 系統類型不進行屏蔽
                    if (filedType.Namespace == nameof(System)) {
                        continue;
                    }

                    // 取值
                    var value = filed.GetValue(data);
                    if (value == null) continue;

                    // 遞迴屏蔽子項目，並重設欄位值
                    filed.SetValue(
                        data,
                        InternalMask(
                            filedType,
                            value,
                            controller,
                            patternName,
                            refList
                        )
                    );
                }
            }
            #endregion

            return data;
        }
    }
}
