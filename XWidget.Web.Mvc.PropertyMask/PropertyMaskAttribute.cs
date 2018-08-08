using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Web.Mvc.PropertyMask {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PropertyMaskAttribute : Attribute {
        /// <summary>
        /// 屏蔽配對方式
        /// </summary>
        public MaskMethod Method { get; set; } = MaskMethod.PatternName;

        /// <summary>
        /// 配對對象名稱或類型
        /// </summary>
        public object Key { get; private set; }

        /// <summary>
        /// 在<see cref="Method"/>為<see cref="MaskMethod.Controller"/>、<see cref="MaskMethod.PackageType"/>或<see cref="MaskMethod.ReturnType"/>情況下使否要使用匹配繼承鏈，
        /// 並檢查Interfaces
        /// </summary>
        public bool Inherited { get; set; } = true;

        /// <summary>
        /// 直接屏蔽
        /// </summary>
        public PropertyMaskAttribute() { }

        /// <summary>
        /// 屏蔽類型
        /// </summary>
        /// <param name="key">類型</param>
        public PropertyMaskAttribute(Type key) {
            this.Key = key;
        }

        /// <summary>
        /// 屏蔽模式
        /// </summary>
        /// <param name="key">模式名稱</param>
        public PropertyMaskAttribute(string key) {
            this.Key = key;
        }

        /// <summary>
        /// 檢查是否匹配
        /// </summary>
        /// <param name="controller">控制器實例</param>
        /// <param name="declaringType">定義類型</param>
        /// <param name="packageType">包裝類型</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns></returns>
        internal bool IsMatch(Controller controller, Type declaringType, Type packageType, string patternName) {
            if (Key == null) {
                return true;
            }
            switch (Method) {
                case MaskMethod.Controller:
                    if (Inherited) {
                        return GetAllRefTypes(controller.GetType()).Contains(Key);
                    } else {
                        return Key.Equals(controller.GetType());
                    }
                case MaskMethod.DeclaringType:
                    if (Inherited) {
                        return GetAllRefTypes(declaringType).Contains(Key);
                    } else {
                        return Key.Equals(declaringType);
                    }
                case MaskMethod.PackageType:
                    if (Inherited) {
                        return GetAllRefTypes(packageType).Contains(Key);
                    } else {
                        return Key.Equals(packageType);
                    }
                case MaskMethod.PatternName:
                    return Key.Equals(patternName);
                case MaskMethod.ActionName:
                    return Key.Equals(controller.ControllerContext.ActionDescriptor.MethodInfo.Name);
                case MaskMethod.ReturnType:
                    if (Inherited) {
                        return GetAllRefTypes(controller.ControllerContext.ActionDescriptor.MethodInfo.ReturnType).Contains(Key);
                    } else {
                        return Key.Equals(controller.ControllerContext.ActionDescriptor.MethodInfo.ReturnType);
                    }
            }

            return false;
        }

        /// <summary>
        /// 取得所有繼承類型
        /// </summary>
        /// <param name="type">類型</param>
        /// <returns>類型陣列</returns>
        private Type[] GetBaseTypes(Type type) {
            if (type == typeof(object)) {
                return new Type[] { };
            }
            if (type.BaseType == null) {
                return new Type[] { type };
            }
            return new Type[] { type }.Concat(GetBaseTypes(type.BaseType)).ToArray();
        }

        /// <summary>
        /// 取得關聯的所有型別
        /// </summary>
        /// <param name="type">類型</param>
        /// <returns>類型陣列</returns>
        private Type[] GetAllRefTypes(Type type) {
            return GetBaseTypes(type).Concat(type.GetInterfaces()).ToArray();
        }
    }
}
