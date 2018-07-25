using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace XWidget.EFLogic {
    /// <summary>
    /// 動態邏輯處理對應
    /// </summary>
    public class DynamicLogicMap<TContext>
        where TContext : DbContext {
        /// <summary>
        /// 型別與主鍵對應
        /// </summary>
        public Dictionary<Type, string> Maps { get; set; }
            = new Dictionary<Type, string>();

        /// <summary>
        /// 本次執行階段產生的LogicBase實例
        /// </summary>
        private Dictionary<Type, object> runtimeLogicMaps { get; set; }
            = new Dictionary<Type, object>();

        /// <summary>
        /// 取得或建立指定型別的Logic
        /// </summary>
        /// <param name="logicManager">邏輯管理器</param>
        /// <param name="type">型別</param>
        /// <returns>指定型別的Logic實例</returns>
        public object GetLogicByType(LogicManagerBase<TContext> logicManager, Type type) {
            if (!runtimeLogicMaps.ContainsKey(type)) {
                CreateLogicByType(logicManager, type);
            }
            return runtimeLogicMaps[type];
        }

        private void CreateLogicByType(LogicManagerBase<TContext> logicManager, Type type) {
            if (!Maps.ContainsKey(type)) {
                throw new NotSupportedException($"Not support type {type.Name}");
            }

            //建構組件
            AssemblyBuilder tempAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName() {
                Name = "TempAssembly_" + Guid.NewGuid().ToString().Replace('-', '_')
            }, AssemblyBuilderAccess.RunAndCollect);

            //建構模組
            ModuleBuilder tempModuleBuilder = tempAssemblyBuilder
                .DefineDynamicModule("TempModule_" + Guid.NewGuid().ToString().Replace('-', '_'));


            var logicType = typeof(LogicBase<,,>).MakeGenericType(typeof(TContext), type, type.GetProperty(Maps[type]).PropertyType);


            //建構實作介面類別
            TypeBuilder tempTypeBuilder = tempModuleBuilder.DefineType(
                $"Anon_{Guid.NewGuid().ToString().Replace("-", "_")}",
                TypeAttributes.Class,
                logicType, Type.EmptyTypes);

            CreatePassThroughConstructors(tempTypeBuilder, logicType);

            var tempType = tempTypeBuilder.CreateTypeInfo();
            var ctor = tempType.GetConstructors().Single(x => x.GetParameters().Length > 1 && x.GetParameters()[1].ParameterType == typeof(string));

            runtimeLogicMaps[type]
                = ctor.Invoke(new object[] { logicManager, Maps[type] });
        }

        private void CreatePassThroughConstructors(TypeBuilder builder, Type baseType) {
            foreach (var constructor in baseType.GetConstructors()) {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false)) {
                    //throw new InvalidOperationException("Variadic constructors are not supported");
                    continue;
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
                var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

                var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
                for (var i = 0; i < parameters.Length; ++i) {
                    var parameter = parameters[i];
                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0) {
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);
                    }

                    foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData())) {
                        parameterBuilder.SetCustomAttribute(attribute);
                    }
                }

                foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData())) {
                    ctor.SetCustomAttribute(attribute);
                }

                var emitter = ctor.GetILGenerator();
                emitter.Emit(OpCodes.Nop);

                // Load `this` and call base constructor with arguments
                emitter.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i) {
                    emitter.Emit(OpCodes.Ldarg, i);
                }
                emitter.Emit(OpCodes.Call, constructor);

                emitter.Emit(OpCodes.Ret);
            }
        }

        private CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes) {
            return customAttributes.Select(attribute => {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }
    }
}