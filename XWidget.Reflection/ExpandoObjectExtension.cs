using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace XWidget.Reflection {
    /// <summary>
    /// 針對<see cref="ExpandoObject"/>相關擴充方法
    /// </summary>
    public static class ExpandoObjectExtension {
        /// <summary>
        /// 使用目標實例建立類別物件並繼承指定介面類別
        /// </summary>
        /// <typeparam name="T">指定介面類別</typeparam>
        /// <param name="obj">目標實例</param>
        /// <returns>類別物件</returns>
        public static Type CreateAnonymousType<T>(this ExpandoObject obj) {
            return CreateAnonymousType(obj, typeof(T));
        }

        /// <summary>
        /// 使用目標實例建立類別物件並繼承指定介面類別。值得注意的是，obj內為iType的方法成員者，方法第一個參數為this
        /// </summary>
        /// <param name="obj">目標實例</param>
        /// <param name="interfaceType">指定介面類別</param>
        /// <returns>類別物件</returns>
        public static Type CreateAnonymousType(this ExpandoObject obj, Type interfaceType) {
            //建構組件
            AssemblyBuilder tempAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName() {
                Name = "TempAssembly"
            }, AssemblyBuilderAccess.RunAndCollect);

            //建構模組
            ModuleBuilder tempModuleBuilder = tempAssemblyBuilder.DefineDynamicModule("TempModule");

            //建構實作介面類別
            TypeBuilder tempTypeBuilder = tempModuleBuilder.DefineType(
                $"Anon_{Guid.NewGuid().ToString().Replace("-", "_")}",
                TypeAttributes.Class,
                typeof(object), Type.EmptyTypes);

            //實作介面
            if (interfaceType != null) {
                tempTypeBuilder.AddInterfaceImplementation(interfaceType);
            }

            var dict = obj as IDictionary<string, object>;
            if (dict == null) return null;

            //取得iType的所有Methods
            var methods = interfaceType?.GetMethods() ?? new MethodInfo[0];

            Dictionary<string, object> settingStaticValues = new Dictionary<string, object>();

            foreach (var keyvalue in dict) {
                var propertyType = keyvalue.Value?.GetType() ?? typeof(object);

                var method = methods.SingleOrDefault(x => x.Name == keyvalue.Key);

                if (method != null) {//method body (delegate)
                    FieldBuilder delegateField = tempTypeBuilder.DefineField(
                        "_" + keyvalue.Key,
                        typeof(Delegate),
                        FieldAttributes.Private | FieldAttributes.Static);

                    //加入靜態欄位值，以便建立型別後重新寫入
                    settingStaticValues.Add(delegateField.Name, keyvalue.Value);

                    //取得interface Method參數
                    var parameters = method.GetParameters();

                    //建構方法
                    MethodBuilder tempMethodBuilder =
                        tempTypeBuilder.DefineMethod(method.Name,
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        method.ReturnType,
                        parameters.Select(x => x.ParameterType).ToArray());

                    //使用IL產生器
                    ILGenerator il = tempMethodBuilder.GetILGenerator();

                    //Load Delegate Field To Stack
                    il.Emit(OpCodes.Ldsfld, delegateField);

                    #region Method Invoke Parameters Array
                    il.Emit(OpCodes.Ldc_I4, parameters.Length + 1);
                    il.Emit(OpCodes.Newarr, typeof(object));

                    for (int i = 0; i <= parameters.Length; i++) {
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Ldc_I4, i);
                        il.Emit(OpCodes.Ldarg, i);
                        il.Emit(OpCodes.Box, typeof(object));
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                    #endregion

                    //Call Method And Push Result To Stack Top             
                    il.Emit(OpCodes.Callvirt, typeof(Delegate).GetMember("DynamicInvoke")?.First() as MethodInfo);

                    //Return Stack Top
                    il.Emit(OpCodes.Ret);

                    tempTypeBuilder.DefineMethodOverride(//Override Interface Define Method
                        tempMethodBuilder,
                        method
                    );
                } else {//普通的值轉換為屬性
                    PropertyBuilder property = tempTypeBuilder.DefineProperty(
                        keyvalue.Key,
                        PropertyAttributes.HasDefault,
                        propertyType,
                        null);

                    FieldBuilder field = tempTypeBuilder.DefineField("_" + keyvalue.Key, propertyType, FieldAttributes.Private);

                    MethodAttributes getSetAttr = MethodAttributes.Public |
                        MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                    MethodBuilder mbNumberGetAccessor = tempTypeBuilder.DefineMethod(
                        "get_" + keyvalue.Key,
                        getSetAttr,
                        propertyType,
                        Type.EmptyTypes);

                    ILGenerator numberGetIL = mbNumberGetAccessor.GetILGenerator();
                    numberGetIL.Emit(OpCodes.Ldarg_0);
                    numberGetIL.Emit(OpCodes.Ldfld, field);
                    numberGetIL.Emit(OpCodes.Ret);

                    MethodBuilder mbNumberSetAccessor = tempTypeBuilder.DefineMethod(
                        "set_" + keyvalue.Key,
                        getSetAttr,
                        typeof(void),
                        new Type[] { typeof(object) });

                    ILGenerator numberGetIL2 = mbNumberSetAccessor.GetILGenerator();
                    numberGetIL2.Emit(OpCodes.Ldarg_0);
                    numberGetIL2.Emit(OpCodes.Ldarg_1);
                    numberGetIL2.Emit(OpCodes.Stfld, field);
                    numberGetIL2.Emit(OpCodes.Ret);

                    property.SetSetMethod(mbNumberSetAccessor);
                    property.SetGetMethod(mbNumberGetAccessor);
                }
            }


            var eqObject = (MethodInfo)new Object().GetMember(x => x.Equals(null));
            var eqMethod = tempTypeBuilder.DefineMethod("Equals", eqObject.Attributes, typeof(bool), new Type[] { typeof(object) });
            var eqMethodILG = eqMethod.GetILGenerator();
            eqMethodILG.Emit(OpCodes.Ldarg_0);
            eqMethodILG.Emit(OpCodes.Ldarg_1);
            eqMethodILG.Emit(OpCodes.Call, typeof(ExpandoObjectExtension).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static));
            eqMethodILG.Emit(OpCodes.Ret);

            tempTypeBuilder.DefineMethodOverride(eqMethod, eqObject);


            var hashObject = typeof(object).GetMethod("GetHashCode");
            var hashMethod = tempTypeBuilder.DefineMethod("GetHashCode", hashObject.Attributes, typeof(int), Type.EmptyTypes);
            var hashMethodILG = hashMethod.GetILGenerator();
            hashMethodILG.Emit(OpCodes.Ldarg_0);
            hashMethodILG.Emit(OpCodes.Call, typeof(ExpandoObjectExtension).GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Static));
            hashMethodILG.Emit(OpCodes.Ret);

            tempTypeBuilder.DefineMethodOverride(hashMethod, hashObject);

            var resultType = tempTypeBuilder.CreateTypeInfo();

            //delegateSetting  Method Body Setting
            foreach (var settingkv in settingStaticValues) {
                resultType.GetField(settingkv.Key, BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, settingkv.Value);
            }

            return resultType;
        }

        /// <summary>
        /// 使用目標實例建立類別物件
        /// </summary>
        /// <param name="obj">目標實例</param>
        /// <returns>類別物件</returns>
        public static Type CreateAnonymousType(this ExpandoObject obj) {
            return CreateAnonymousType(obj, null);
        }

        public static int GetHashCode(object obj) {
            var values = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(x => x.GetValue(obj).GetHashCode()).ToArray();
            unchecked {
                int result = 0;
                foreach (int value in values) result += value;
                return result;
            }
        }

        public static bool Equals(object obj1, object obj2) {
            if (obj1 == null || obj2 == null) return false;

            var fields1 = obj1.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(x => x.GetValue(obj1)).ToArray();
            var fields2 = obj1.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(x => x.GetValue(obj2)).ToArray();

            if (fields1.Length != fields2.Length) return false;

            for (int i = 0; i < fields1.Length; i++) {
                if (!fields1[i].Equals(fields2[i])) return false;
            }

            return true;
        }
    }
}
