using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using XWidget.Web.Mvc.JsonMask;

namespace Microsoft.AspNetCore.Mvc {
    public static class ControllerExtension {
        /// <summary>
        /// 回傳遮蔽後的JSON結果
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="data">資料</param>
        /// <param name="patternName">模式名稱</param>
        /// <param name="jsonSerializerSettings">JSON序列化設定</param>
        /// <returns>遮蔽後的JSON結果</returns>
        public static JsonResult JsonByMask<T>(
            this Controller controller,
            T data,
            string patternName = null,
            JsonSerializerSettings jsonSerializerSettings = null) {

            // 檢查是否有自訂序列化設定選項
            if (jsonSerializerSettings == null) {
                // 直接調用預設的設定序列化
                return controller.Json(
                    controller.Mask(data, patternName));
            } else {
                // 加入自訂序列化設定選項
                return controller.Json(
                   controller.Mask(data, patternName),
                   jsonSerializerSettings);
            }
        }

        /// <summary>
        /// 建立屬性遮罩
        /// </summary>
        /// <param name="controller">控制器屬性</param>
        /// <param name="declaringType">定義類型</param>
        /// <param name="type">類型</param>
        /// <param name="patternName">模式名稱</param>
        /// <param name="resolver">JsonMask序列化處理器</param>
        /// <param name="recRecode">方法調用參數紀錄</param>
        private static void AddPropertyMask(
            this Controller controller,
            Type declaringType,
            Type type,
            string patternName,
            PropertyMaskSerializerContractResolver resolver,
            List<(Type declaringType, Type type, string patternName)> recRecode = null) {
            if (recRecode == null) {
                recRecode = new List<(Type declaringType, Type type, string patternName)>();
            }

            var interfaces = type.GetInterfaces();
            // 檢驗類型是否為列舉類型
            if (interfaces.Any(x => x == typeof(IEnumerable))) {
                // 檢驗是否為Array
                if (type.IsArray) {
                    // 上層類型
                    declaringType = type;
                    type = type.GetElementType();
                } else {
                    // 尋找IEnumerable<T>的類型
                    var enumType = interfaces.FirstOrDefault(x => {
                        var nInterfaces = x.GetInterfaces();
                        return nInterfaces.Length == 1 && nInterfaces[0] == typeof(IEnumerable);
                    });
                    // 如果找到該類型
                    if (enumType != null) {
                        // 取出其泛型作為type
                        declaringType = type;
                        type = enumType.GetGenericArguments().First();
                    }
                }
            }

            // 如果類型屬於System的則不做處理
            if (type.Namespace == nameof(System)) {
                return;
            }

            // 檢查參數重複
            if (recRecode.Contains((declaringType, type, patternName))) {
                return;
            }
            // 加入紀錄
            recRecode.Add((declaringType, type, patternName));

            // 取得該類型中非靜態的所有屬性
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                // 取得該屬性的JsonPropertyMaskAttribute集合，如果未設定則應該為空集合
                var attrs = property.GetCustomAttributes<JsonPropertyMaskAttribute>();

                // 在JsonMask設定集合中尋找是否有符合項目
                if (attrs.Any(x => x.IsMatch(controller, declaringType, patternName))) {
                    // 找到符合項目則設定屏蔽
                    resolver.MaskProperty(type, property.Name);
                } else {
                    // 該屬性找不到屏蔽設定，檢查該屬性的屬性類型是否有屏蔽選項
                    var propertyType = property.PropertyType;

                    // 如果是列舉類型
                    if (propertyType.GetInterfaces().Any(x => x == typeof(IEnumerable))) {
                        // 遞迴檢查屬性類型是否有屏蔽項目
                        AddPropertyMask(controller, propertyType, propertyType, patternName, resolver, recRecode);
                    } else {
                        // 遞迴檢查屬性類型是否有屏蔽項目
                        AddPropertyMask(controller, type, propertyType, patternName, resolver, recRecode);
                    }
                }
            }

            // 取得該類型中非靜態的所有欄位
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                // 取得該欄位的JsonPropertyMaskAttribute集合，如果未設定則應該為空集合
                var attrs = field.GetCustomAttributes<JsonPropertyMaskAttribute>();

                // 在JsonMask設定集合中尋找是否有符合項目
                if (attrs.Any(x => x.IsMatch(controller, declaringType, patternName))) {
                    // 找到符合項目則設定屏蔽
                    resolver.MaskProperty(type, field.Name);
                } else {
                    // 該欄位找不到屏蔽設定，檢查該欄位的屬性類型是否有屏蔽選項
                    var fieldType = field.FieldType;

                    // 如果是列舉類型
                    if (fieldType.GetInterfaces().Any(x => x == typeof(IEnumerable))) {
                        // 遞迴檢查屬性類型是否有屏蔽項目
                        AddPropertyMask(controller, fieldType, fieldType, patternName, resolver, recRecode);
                    } else {
                        // 遞迴檢查屬性類型是否有屏蔽項目
                        AddPropertyMask(controller, type, fieldType, patternName, resolver, recRecode);
                    }
                }
            }
        }

        /// <summary>
        /// 取得屏蔽過濾後的資料
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="source">原始資料</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns>屏蔽過濾後的資料</returns>
        public static T Mask<T>(this Controller controller, T source, string patternName = null) {
            // 建立JsonMask序列化處理器
            var resolvers = new PropertyMaskSerializerContractResolver();

            // 建立屬性遮罩
            AddPropertyMask(controller, source.GetType(), source.GetType(), patternName, resolvers);

            // 建立序列化設定選項
            var serializerSettings = new JsonSerializerSettings();

            // 設定處理器
            serializerSettings.ContractResolver = resolvers;

            // 將原始資料透過JsonMask序列化處理器序列化後在反序列化回來，此時已經去掉屏蔽欄位，Mask後的物件與原本物件沒有參考關係
            return JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(source, serializerSettings));
        }
    }
}
