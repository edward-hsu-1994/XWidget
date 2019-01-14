using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XWidget.Web {
    /// <summary>
    /// 針對JSON.Net的DefaultContractResolver項目產生JSON屬性擴充方法
    /// </summary>
    public static class ContractResolverExtension {
        /// <summary>
        /// 透過ModelMetadataTypeAttribute中定義的MetadataType作為JSON Object屬性參考原因
        /// </summary> 
        public static IList<JsonProperty> CreatePropertiesWithModelMetadataType(this DefaultContractResolver @base, Type type, MemberSerialization memberSerialization) {
            var createPropertiesMethods = typeof(DefaultContractResolver).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var createPropertiesMethod = createPropertiesMethods.SingleOrDefault(x => x.GetParameters().Select(y => y.ParameterType).Except(new Type[] { typeof(Type), typeof(MemberSerialization) }).Count() == 0);

            IList<JsonProperty> properties = ((IList<JsonProperty>)createPropertiesMethod.Invoke(@base, new object[] { type, memberSerialization })).ToArray();

            var metaType = type.GetCustomAttribute<ModelMetadataTypeAttribute>()?.MetadataType;
            if (metaType == null) {
                return properties;
            }

            var metaProperties = CreatePropertiesWithModelMetadataType(@base, metaType, memberSerialization);

            for (int i = 0; i < properties.Count; i++) {
                var metaProp = metaProperties.SingleOrDefault(x => x.PropertyName == properties[i].PropertyName);
                if (metaProp == null) continue;

                properties[i] = metaProp; // 替換
            }

            return properties;
        }

        /// <summary>
        /// 建立屬性時忽略EntityFrameworkCore使用的LazyLoader屬性
        /// </summary>
        public static JsonProperty CreatePropertyWithIgnoreLazyLoader(this DefaultContractResolver @base, MemberInfo member, MemberSerialization memberSerialization) {
            var createPropertyMethod = typeof(DefaultContractResolver).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Single(x => x.Name == "CreateProperty");

            JsonProperty prop = (JsonProperty)createPropertyMethod.Invoke(@base, new object[] { member, memberSerialization });

            if (member is PropertyInfo property) {
                if (property.PropertyType == typeof(ILazyLoader) ||
                    property.PropertyType.GetInterfaces().Contains(typeof(ILazyLoader))) {

                    prop.Ignored = true;
                }
            }

            return prop;
        }
    }
}
