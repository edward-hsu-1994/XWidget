using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web.Mvc.Multipart {
    /// <summary>
    /// 模型綁定器與提供者
    /// </summary>
    public class MultipartJsonModelBinderProvider : IModelBinderProvider, IModelBinder {
        /// <summary>
        /// 嘗試取得相應的模型綁定器
        /// </summary>
        /// <param name="context">綁定內容</param>
        /// <returns>模型綁定器實例</returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));


            if (context.Metadata.IsComplexType) {
                var propName = context.Metadata.PropertyName;
                var propInfo = context.Metadata.ContainerType?.GetProperty(propName);
                var attribute = propInfo
                    ?.GetCustomAttributes(typeof(FromJsonAttribute), false)
                    ?.FirstOrDefault();

                // 直接將整個FormData綁定至單一複雜類型的參數上
                if (propName != null && propInfo != null && attribute != null) {
                    return new MultipartJsonModelBinderProvider();
                }

                if (context?.Metadata?.BinderType?.GetCustomAttribute<FromJsonAttribute>() != null) {
                    return new MultipartJsonModelBinderProvider();
                }

                var visited = context.GetType().GetProperty("Visited", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                ControllerParameterDescriptor cpd = null;
                foreach (var item in (IEnumerable)visited.GetValue(context)) {
                    var temp = item.GetType().GetProperty("Key").GetValue(item);
                    var tokenField = temp.GetType().GetField("_token", BindingFlags.Instance | BindingFlags.NonPublic);
                    cpd = tokenField.GetValue(temp) as ControllerParameterDescriptor;

                    if (cpd != null) break;
                }

                if (cpd != null && cpd.ParameterInfo.GetCustomAttribute<FromJsonAttribute>() != null) {
                    return new MultipartJsonModelBinderProvider();
                }
            }

            return null;
        }

        /// <summary>
        /// 綁定資料
        /// </summary>
        /// <param name="bindingContext">綁定內容</param>
        public async Task BindModelAsync(ModelBindingContext bindingContext) {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != ValueProviderResult.None) {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.FirstValue;

                var result = JsonConvert.DeserializeObject(valueAsString, bindingContext.ModelType);

                if (result != null) {
                    bindingContext.Result = ModelBindingResult.Success(result);
                }
            }
        }

    }
}
