using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace XWidget.Web {
    public class CommonContractResolver : CamelCasePropertyNamesContractResolver {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            var metaType = type.GetCustomAttribute<ModelMetadataTypeAttribute>()?.MetadataType;
            if (metaType == null) {
                return properties;
            }

            var metaProperties = CreateProperties(metaType, memberSerialization);

            for (int i = 0; i < properties.Count; i++) {
                var metaProp = metaProperties.SingleOrDefault(x => x.PropertyName == properties[i].PropertyName);
                if (metaProp == null) continue;

                properties[i] = metaProp; // 替換
            }

            return properties;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

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
