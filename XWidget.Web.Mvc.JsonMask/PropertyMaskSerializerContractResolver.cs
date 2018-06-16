using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace XWidget.Web.Mvc.JsonMask {
    /// <summary>
    /// JsonMask序列化處理器
    /// </summary>
    internal class PropertyMaskSerializerContractResolver : DefaultContractResolver {
        /// <summary>
        /// 屏蔽屬性列表
        /// </summary>
        Dictionary<Type, HashSet<string>> masks = new Dictionary<Type, HashSet<string>>();

        /// <summary>
        /// 屏蔽指定類型的屬性
        /// </summary>
        /// <param name="type">類型</param>
        /// <param name="jsonPropertyNames">屬性名稱</param>
        public void MaskProperty(Type type, params string[] jsonPropertyNames) {
            // 檢驗是否已經存在該類型
            if (!masks.ContainsKey(type)) {
                // 建立屏蔽類型屬性集合物件
                masks[type] = new HashSet<string>();
            }

            /// 加入屏蔽屬性集合中
            foreach (var prop in jsonPropertyNames) {
                masks[type].Add(prop);
            }
        }

        /// <summary>
        /// 檢驗指令屬性在特定類型中是否已經被屏蔽
        /// </summary>
        /// <param name="type">目標類型</param>
        /// <param name="propertyName">屬性名稱</param>
        /// <returns>是否已經被屏蔽</returns>
        private bool IsMasked(Type type, string propertyName) {
            // 檢驗該類型是否擁有屏蔽屬性集合
            if (!masks.ContainsKey(type)) {
                return false;
            }

            // 檢驗該類型屏蔽屬性集合是否存在該屬性
            return masks[type].Contains(propertyName);
        }

        /// <summary>
        /// 複寫 <see cref="DefaultContractResolver.CreateProperty(MemberInfo, MemberSerialization)"/>
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            var property = base.CreateProperty(member, memberSerialization);

            // 檢驗該屬性是否已經被屏蔽
            if (IsMasked(property.DeclaringType, member.Name)) {
                // 設定該屬性為不可序列化
                property.ShouldSerialize = i => false;
            }

            return property;
        }
    }
}
