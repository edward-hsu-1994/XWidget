using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XWidget.Web.Mvc.EFETag {
    public delegate bool ResponseCacheProcess(string etag, HttpContext context);
    public class EFETagMiddleware<TContext>
        where TContext : DbContext {
        private static bool Inited = false;

        public static ConcurrentDictionary<string, string> ETags = new ConcurrentDictionary<string, string>();
        public static Dictionary<string, Type[]> ActionRefType = new Dictionary<string, Type[]>();

        private readonly RequestDelegate Next;

        public static event EventHandler ETagInit;
        public static event EventHandler<KeyValuePair<string, string>> ETagUpdated;
        public static ResponseCacheProcess LoadResponseCache { get; set; }
        public static ResponseCacheProcess SaveResponseCache { get; set; }

        public EFETagMiddleware(RequestDelegate next) {
            Next = next;
        }

        public static void UpdateETag(string type, string value, bool ignorePublish = false) {
            ETags[type] = value;
            if (!ignorePublish) {
                ETagUpdated?.Invoke(ETags, ETags.First(x => x.Key == type));
            }
        }

        public async Task Invoke(HttpContext context) {
            #region 初次運行
            if (!Inited) {
                #region 取得Entity Type
                var modelTypes = typeof(TContext)
                    .GetProperties()
                    .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(x => x.PropertyType.GenericTypeArguments[0]).Distinct();

                foreach (var modelType in modelTypes) {
                    ETags[modelType.Name] = DateTime.Now.Ticks.ToString();
                }

                ETagInit?.Invoke(this, null);
                #endregion

                // 取出所有MVC路由
                var actionMethods = (
                        (IActionDescriptorCollectionProvider)context
                        .RequestServices.GetService(typeof(IActionDescriptorCollectionProvider))
                    )
                    .ActionDescriptors.Items.Where(x => x is ControllerActionDescriptor)
                    .Cast<ControllerActionDescriptor>()
                    .Select(x => x.MethodInfo);

                #region 獲取每個Action回傳值關聯的Model
                Type[] GetAllBaseTypes(Type type) {
                    List<Type> result = new List<Type>();

                    if (type.BaseType != null && type.BaseType != typeof(object)) {
                        result.Add(type.BaseType);
                        result.AddRange(GetAllBaseTypes(type.BaseType));
                    }

                    return result.ToArray();
                }

                List<Type> GetAllJsonType(Type type, List<Type> result = null) {
                    if (result == null) {
                        result = new List<Type>();
                    }

                    if (result.Contains(type)) {
                        return result;
                    }

                    var jsonObjectDef = type.GetCustomAttribute<JsonObjectAttribute>();
                    if (jsonObjectDef == null) {
                        jsonObjectDef = new JsonObjectAttribute() { MemberSerialization = MemberSerialization.OptOut };
                    }

                    result.Add(type);
                    result.AddRange(GetAllBaseTypes(type));

                    if (type.IsGenericType) {
                        foreach (var gType in type.GenericTypeArguments) {
                            result.AddRange(GetAllJsonType(gType, result));
                        }
                    }

                    if (result.Contains(type)) {
                        return result;
                    }

                    object tempObj = null;

                    try {
                        tempObj = FormatterServices.GetUninitializedObject(type);
                    } catch { }


                    if (jsonObjectDef.MemberSerialization == MemberSerialization.Fields) {
                        result.AddRange(
                            type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                .Select(x => x.FieldType)
                                .SelectMany(x => GetAllJsonType(type, result))
                        );
                    } else if (jsonObjectDef.MemberSerialization == MemberSerialization.OptIn) {
                        result.AddRange(
                            type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                .Where(x => x.GetCustomAttribute<JsonPropertyAttribute>() != null)
                                .Where(x => {
                                    var ssm = type.GetMethod("ShouldSerialize" + x.Name);
                                    if (ssm == null) return true;
                                    if (tempObj == null) return true;
                                    return true.Equals(ssm.Invoke(tempObj, new object[0]));
                                })
                                .Select(x => x.PropertyType)
                                .Where(x => x.IsClass && x != typeof(string))
                                .SelectMany(x => GetAllJsonType(type, result))
                        );
                    } else if (jsonObjectDef.MemberSerialization == MemberSerialization.OptOut) {
                        var pp = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                .Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                                .Where(x => {
                                    var ssm = type.GetMethod("ShouldSerialize" + x.Name);
                                    if (ssm == null) return true;
                                    if (tempObj == null) return true;
                                    return true.Equals(ssm.Invoke(tempObj, new object[0]));
                                })
                                .Select(x => x.PropertyType)
                                .Where(x => x.IsClass && x != typeof(string))
                                .ToArray();

                        result.AddRange(
                            type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                .Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                                .Where(x => {
                                    var ssm = type.GetMethod("ShouldSerialize" + x.Name);
                                    if (ssm == null) return true;
                                    if (tempObj == null) return true;
                                    return true.Equals(ssm.Invoke(tempObj, new object[0]));
                                })
                                .Select(x => x.PropertyType)
                                .Where(x => x.IsClass && x != typeof(string))
                                .SelectMany(x => GetAllJsonType(type, result))
                        );
                    }

                    return result;
                }

                foreach (var actionMethod in actionMethods) {
                    var actionId = $"{actionMethod.DeclaringType.FullName}.{actionMethod.Name}<{string.Join(", ", actionMethod.GetParameters().Select(x => x.ParameterType.FullName))}>".ToLower();

                    if (actionMethod.ReturnType == null) {
                        continue;
                    }

                    ActionRefType[actionId] = GetAllJsonType(actionMethod.ReturnType).Distinct()
                        .Intersect(modelTypes).OrderBy(x => x.Name).ToArray();
                }
                #endregion

                Inited = true;
            }
            #endregion

            #region 追蹤DBContext異動
            var dbcontext = (TContext)context.RequestServices.GetService(typeof(TContext));
            // 異動ModelTypes
            ConcurrentBag<Type> modifiedTypes = new ConcurrentBag<Type>();

            // Model狀態變化Handler
            EventHandler<EntityStateChangedEventArgs> eventHandler =
                delegate (object sender, EntityStateChangedEventArgs e) {
                    // 重複略過
                    if (modifiedTypes.Contains(e.Entry.Metadata.ClrType)) return;

                    // 舊狀態為Added則表示為剛新增的Instance
                    // 變更以及被刪除的項目加入異動項目
                    if (e.OldState == EntityState.Added ||
                    e.NewState == EntityState.Modified ||
                    e.NewState == EntityState.Deleted) {
                        modifiedTypes.Add(e.Entry.Metadata.ClrType);
                    }
                };

            // 加入事件綁定
            dbcontext.ChangeTracker.StateChanged += eventHandler;
            #endregion

            #region 抓出目前Request的作用Action MethodInfo
            // 取出所有MVC路由
            var actions = (
                    (IActionDescriptorCollectionProvider)context.RequestServices.GetService(typeof(IActionDescriptorCollectionProvider))
                )
                .ActionDescriptors.Items.Where(x => x is ControllerActionDescriptor)
                .Cast<ControllerActionDescriptor>();

            // 找出目前作用中的Action的MethodInfo
            var action = actions.FirstOrDefault(x => {
                var templateUrl = ("^/" + Regex.Replace(x.AttributeRouteInfo.Template, "\\{[^\\{\\}]+\\}", ".+") + "/?$")
                        .ToLower();

                var isMatch = new Regex(templateUrl).IsMatch(context.Request.Path.ToString().ToLower());

                if (!isMatch) return false;

                var actionConstraint = x.ActionConstraints.FirstOrDefault(y => y is HttpMethodActionConstraint) as HttpMethodActionConstraint;

                return actionConstraint.HttpMethods.Any(y => context.Request.Method.Equals(y, StringComparison.CurrentCultureIgnoreCase));
            })?.MethodInfo;
            #endregion

            // 如果目前Request為針對MVC方法
            if (HttpMethods.IsGet(context.Request.Method) &&// 必須為GET
                action != null) {// 且包含舊的ETag
                // 目前Action的識別
                var currentActionId = $"{action.DeclaringType.FullName}.{action.Name}<{string.Join(", ", action.GetParameters().Select(x => x.ParameterType.FullName))}>".ToLower();

                var etag = string.Join(",", ActionRefType[currentActionId].Select(x => ETags[x.Name])).ToHashString<MD5>();

                // 透過相關ModelType的ETag計算MD5作為ETag
                var currentETag = "W/\"" + etag + "\"";

                if (context.Request.Headers.TryGetValue("If-None-Match", out StringValues oldETag)) {
                    // ETag相等於計算的，表示內容無變化，返回304
                    if (oldETag[0] == currentETag) {
                        context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                        return;
                    }
                } else {
                    // 讀取快取
                    if (LoadResponseCache != null) {
                        if (LoadResponseCache.Invoke(
                            currentETag,
                            context)) {
                            return;
                        }
                    }
                }
            }

            await Next(context);

            // 如果目前Request為針對MVC方法
            if (HttpMethods.IsGet(context.Request.Method) &&// 必須為GET
                action != null) {// 且包含舊的ETag
                // 目前Action的識別
                var currentActionId = $"{action.DeclaringType.FullName}.{action.Name}<{string.Join(", ", action.GetParameters().Select(x => x.ParameterType.FullName))}>".ToLower();

                var etag = string.Join(",", ActionRefType[currentActionId].Select(x => ETags[x.Name])).ToHashString<MD5>();

                // 透過相關ModelType的ETag計算MD5作為ETag
                var currentETag = "W/\"" + etag + "\"";

                // 儲存快取
                if (SaveResponseCache != null) {
                    if (SaveResponseCache.Invoke(
                        currentETag,
                        context)) {
                        return;
                    }
                }
            }

            // 刪除事件綁定
            dbcontext.ChangeTracker.StateChanged -= eventHandler;

            // 更新異動ModelType的ETag標記
            foreach (var modifiedType in modifiedTypes) {
                UpdateETag(modifiedType.Name, DateTime.Now.Ticks.ToString());
            }

            if (HttpMethods.IsGet(context.Request.Method) && // 必須為GET
                action != null) { // 如果調用的是API，則補充該API的
                                  // 目前Action的識別
                var currentActionId = $"{action.DeclaringType.FullName}.{action.Name}<{string.Join(", ", action.GetParameters().Select(x => x.ParameterType.FullName))}>".ToLower();

                // 透過相關ModelType的ETag計算MD5作為ETag
                var currentETag = "W/\"" + string.Join(",", ActionRefType[currentActionId].Select(x => ETags[x.Name])).ToHashString<MD5>() + "\"";

                // 寫入標頭
                context.Response.Headers.Add("ETag", currentETag);
            }
        }
    }
}
