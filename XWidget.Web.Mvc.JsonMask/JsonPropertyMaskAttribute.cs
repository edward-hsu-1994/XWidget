using Microsoft.AspNetCore.Mvc;
using System;

namespace XWidget.Web.Mvc.JsonMask {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class JsonPropertyMaskAttribute : Attribute {
        /// <summary>
        /// 屏蔽配對方式
        /// </summary>
        public MaskMethod Method { get; set; } = MaskMethod.PatternName;

        /// <summary>
        /// 配對對象名稱或類型
        /// </summary>
        public object Key { get; private set; }

        public JsonPropertyMaskAttribute(Type key) {
            this.Key = key;
        }

        public JsonPropertyMaskAttribute(string key) {
            this.Key = key;
        }

        /// <summary>
        /// 檢查是否匹配
        /// </summary>
        /// <param name="controller">控制器實例</param>
        /// <param name="declaringType">回傳類型</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns></returns>
        internal bool IsMatch(Controller controller, Type declaringType, string patternName) {
            switch (Method) {
                case MaskMethod.Controller:
                    return Key.Equals(controller.GetType());
                case MaskMethod.DeclaringType:
                    return Key.Equals(declaringType);
                case MaskMethod.PatternName:
                    return Key.Equals(patternName);
                case MaskMethod.Action:
                    return Key.Equals(controller.ControllerContext.ActionDescriptor.MethodInfo.Name);
                case MaskMethod.ReturnType:
                    return Key.Equals(controller.ControllerContext.ActionDescriptor.MethodInfo.ReturnType);
            }

            return false;
        }
    }
}
