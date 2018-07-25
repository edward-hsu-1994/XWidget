using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XWidget.EFLogic {
    /// <summary>
    /// 動態操作邏輯對應建構器
    /// </summary>
    /// <typeparam name="TContext">資料庫內容類型</typeparam>
    public class DynamicLogicMapBuilder<TContext> where TContext : DbContext {
        /// <summary>
        /// 型別與主鍵對應
        /// </summary>
        public Dictionary<Type, string> Maps { get; private set; }
            = new Dictionary<Type, string>();

        /// <summary>
        /// 加入動態邏輯實例
        /// </summary>
        /// <param name="entityType">類型</param>
        /// <param name="identityName">主鍵名稱</param>
        /// <returns>動態操作邏輯對應建構器</returns>
        public DynamicLogicMapBuilder<TContext> AddDynamicLogic(
            Type entityType,
            string identityName) {

            Maps[entityType] = identityName;

            return this;
        }

        /// <summary>
        /// 加入動態邏輯實例
        /// </summary>
        /// <typeparam name="TEntity">類型</typeparam>
        /// <param name="identityName">主鍵名稱</param>
        /// <returns>動態操作邏輯對應建構器</returns>
        public DynamicLogicMapBuilder<TContext> AddDynamicLogic<TEntity>(
            string identityName) {
            return AddDynamicLogic(typeof(TEntity), identityName);
        }

        /// <summary>
        /// 自DbContext加入所有Entity類型
        /// </summary>
        /// <param name="identityNameFunc">型別指定主鍵方法</param>
        /// <returns>動態操作邏輯對應建構器</returns>
        public DynamicLogicMapBuilder<TContext> AddFromDbContext(Func<Type, string> identityNameFunc) {
            var types = typeof(TContext).GetProperties()
                .Where(x =>
                    x.PropertyType.IsGenericType &&
                    x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                ).Select(x => x.PropertyType.GetGenericArguments()[0]);

            var result = this;
            foreach (var type in types) {
                result = result.AddDynamicLogic(type, identityNameFunc(type));
            }
            return result;
        }

        /// <summary>
        /// 自DbContext加入所有Entity類型
        /// </summary>
        /// <param name="identityName">主鍵屬性名稱</param>
        /// <returns>動態操作邏輯對應建構器</returns>
        public DynamicLogicMapBuilder<TContext> AddFromDbContext(string identityName = "Id") {
            var types = typeof(TContext).GetProperties()
                .Where(x =>
                    x.PropertyType.IsGenericType &&
                    x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                ).Select(x => x.PropertyType.GetGenericArguments()[0]);

            var result = this;
            foreach (var type in types) {
                result = result.AddDynamicLogic(type, identityName);
            }
            return result;
        }
    }
}