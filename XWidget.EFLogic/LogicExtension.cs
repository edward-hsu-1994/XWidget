using XWidget.EFLogic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// 邏輯管理器擴充
    /// </summary>
    public static class LogicExtension {
        /// <summary>
        /// 加入邏輯管理
        /// </summary>
        /// <typeparam name="TLogic">邏輯管理器類型</typeparam>
        /// <typeparam name="TContext">資料庫內容類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <param name="optionsAction">EntityFramework選項</param>
        /// <returns>動態邏輯建構器</returns>
        public static DynamicLogicMapBuilder<TContext> AddLogic<TLogic, TContext>(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
            where TLogic : LogicManagerBase<TContext>
            where TContext : DbContext {
            services.AddDbContext<TContext>(optionsAction);

            var builder = new DynamicLogicMapBuilder<TContext>();

            services.AddScoped<TLogic>(serviceProvider => {
                var constructors = typeof(TLogic).GetConstructors().FirstOrDefault();
                var createServices = constructors.GetParameters().Select(x => x.ParameterType)
                    .Select(x => serviceProvider.GetService(x)).ToArray();
                var instance = (TLogic)constructors.Invoke(createServices);
                instance.MapBuilder = builder;

                return instance;
            });

            return builder;
        }

        /// <summary>
        /// 加入邏輯管理
        /// </summary>
        /// <typeparam name="TLogic">邏輯管理器類型</typeparam>
        /// <typeparam name="TContext">資料庫內容類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <param name="optionsAction">EntityFramework選項</param>
        /// <returns>動態邏輯建構器</returns>
        public static DynamicLogicMapBuilder<TContext> AddLogic<TLogic, TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsAction)
            where TLogic : LogicManagerBase<TContext>
            where TContext : DbContext {
            services.AddDbContext<TContext>(optionsAction);

            var builder = new DynamicLogicMapBuilder<TContext>();

            services.AddScoped<TLogic>(serviceProvider => {
                var constructors = typeof(TLogic).GetConstructors().FirstOrDefault();
                var createServices = constructors.GetParameters().Select(x => x.ParameterType)
                    .Select(x => serviceProvider.GetService(x)).ToArray();
                var instance = (TLogic)constructors.Invoke(createServices);
                instance.MapBuilder = builder;

                return instance;
            });

            return builder;
        }

        /// <summary>
        /// 加入邏輯管理
        /// </summary>
        /// <typeparam name="TLogic">邏輯管理器類型</typeparam>
        /// <typeparam name="TContext">資料庫內容類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <returns>動態邏輯建構器</returns>
        public static DynamicLogicMapBuilder<TContext> AddLogic<TLogic, TContext>(
            this IServiceCollection services)
            where TLogic : LogicManagerBase<TContext>
            where TContext : DbContext {
            services.AddDbContext<TContext>();

            var builder = new DynamicLogicMapBuilder<TContext>();

            services.AddScoped<TLogic>(serviceProvider => {
                var constructors = typeof(TLogic).GetConstructors().FirstOrDefault();
                var createServices = constructors.GetParameters().Select(x => x.ParameterType)
                    .Select(x => serviceProvider.GetService(x)).ToArray();
                var instance = (TLogic)constructors.Invoke(createServices);
                instance.MapBuilder = builder;
                instance.ServiceProvider = serviceProvider;

                List<PropertyInfo> hasLogicProperties = new List<PropertyInfo>();
                Dictionary<Type, object> logicMapping = new Dictionary<Type, object>();

                hasLogicProperties.AddRange(typeof(TLogic).GetProperties());

                while (hasLogicProperties.Any()) {
                    var currentLoopProperties = hasLogicProperties.ToArray();
                    hasLogicProperties.Clear();

                    foreach (var property in currentLoopProperties) {
                        var isLogic = GetAllBaseTypes(property.PropertyType).Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(LogicBase<,,>));

                        if (!isLogic) continue;

                        var constuctor = property.PropertyType.GetConstructors().Single();
                        var constuctorParameters = constuctor.GetParameters();
                        var hasLogicParam = false;

                        List<object> parameterValues = new List<object>();
                        foreach (var param in constuctorParameters) {
                            var isLogicManager = param.ParameterType.GetAllBaseTypes().Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(LogicManagerBase<>));
                            var paramIsLogic = param.ParameterType.GetAllBaseTypes().Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(LogicBase<,,>));

                            if (isLogicManager) {
                                parameterValues.Add(instance);
                            } else if (isLogic) {
                                if (logicMapping.ContainsKey(param.ParameterType)) {
                                    parameterValues.Add(logicMapping[param.ParameterType]);
                                } else {
                                    hasLogicParam = true;
                                    break;
                                }
                            } else {
                                parameterValues.Add(serviceProvider.GetService(param.ParameterType));
                            }
                        }

                        if (hasLogicParam) {
                            hasLogicProperties.Add(property);
                            continue;
                        }

                        var logicPropertyValue = constuctor.Invoke(parameterValues.ToArray());
                        logicMapping[property.PropertyType] = logicPropertyValue;
                        property.SetValue(instance, logicPropertyValue);
                    }
                }

                return instance;
            });

            return builder;
        }

        private static Type[] GetAllBaseTypes(this Type type) {
            if (type == null) {
                return Type.EmptyTypes;
            }
            if (type == typeof(object)) {
                return new Type[] { typeof(object) };
            }
            return new Type[] { type }.Concat(GetAllBaseTypes(type.BaseType)).ToArray();
        }
    }
}