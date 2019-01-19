using AutoCompare;
using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace XWidget.EFLogic {
    /// <summary>
    /// 邏輯管理器
    /// </summary>
    public abstract class LogicManagerBase<TContext> : LogicManagerBase<TContext, object[]>
        where TContext : DbContext {
        /// <summary>
        /// 邏輯管理器建構子
        /// </summary>
        /// <param name="database">資料庫上下文</param>
        public LogicManagerBase(TContext database) : base(database) {
        }
    }

    /// <summary>
    /// 邏輯管理器
    /// </summary>
    public abstract class LogicManagerBase<TContext, TParameters> where TContext : DbContext {
        internal DynamicLogicMapBuilder<TContext> MapBuilder { get; set; }

        /// <summary>
        /// DI服務提供者
        /// </summary>
        public IServiceProvider ServiceProvider { get; internal set; }

        /// <summary>
        /// 資料庫上下文
        /// </summary>
        public TContext Database { get; internal set; }

        /// <summary>
        /// 動態邏輯對應
        /// </summary>
        public DynamicLogicMap<TContext, TParameters> DynamicLogicMap {
            get {
                return new DynamicLogicMap<TContext, TParameters>() {
                    Maps = MapBuilder.Maps
                };
            }
        }

        /// <summary>
        /// 邏輯管理器建構子
        /// </summary>
        /// <param name="database">資料庫上下文</param>
        public LogicManagerBase(TContext database) {
            Database = database;
        }

        /// <summary>
        /// 取得動態類型操作邏輯
        /// </summary>
        /// <typeparam name="TEntity">實例類型</typeparam>
        /// <typeparam name="TId">唯一識別號類型</typeparam>
        /// <returns>操作邏輯</returns>
        public LogicBase<TContext, TEntity, TId, TParameters> GetLogicByType<TEntity, TId>()
            where TEntity : class {
            return (LogicBase<TContext, TEntity, TId, TParameters>)GetLogicByType(typeof(TEntity));
        }

        internal object GetLogicByType(Type type) {
            // EFCore LazyLoad Proxy Support
            if (ProxyUtil.IsProxyType(type)) {
                return GetLogicByType(type.BaseType);
            }

            var prop = this.GetType()
                .GetProperties()
                .SingleOrDefault(
                    x =>
                        (
                            x.PropertyType.IsGenericType &&
                            x.PropertyType.GetGenericTypeDefinition() == typeof(LogicBase<,,,>) &&
                            x.PropertyType.GenericTypeArguments[1] == type
                        ) || (
                            x.PropertyType.BaseType != null &&
                            x.PropertyType.BaseType.IsGenericType &&
                            x.PropertyType.BaseType.GetGenericTypeDefinition() == typeof(LogicBase<,,,>) &&
                            x.PropertyType.BaseType.GenericTypeArguments[1] == type
                        )
                );

            if (prop == null) {
                return DynamicLogicMap.GetLogicByType(this, type);
            }

            return prop.GetValue(this);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="cond">條件</param>
        /// <returns>列表</returns>
        public virtual async Task<IQueryable<TEntity>> ListAsync<TEntity>(
            Expression<Func<TEntity, bool>> cond = null)
            where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic.ListAsync(cond));
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="cond">條件</param>
        /// <returns>列表</returns>
        public virtual IQueryable<TEntity> List<TEntity>(Expression<Func<TEntity, bool>> cond = null)
            where TEntity : class {
            return ListAsync(cond).ToSync();
        }

        /// <summary>
        /// 全文搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <returns>搜尋結果</returns>
        public virtual async Task<IQueryable<TEntity>> SearchAsync<TEntity>(
            string likePatten)
            where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic.SearchAsync(likePatten));
        }

        /// <summary>
        /// 全文搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <returns>搜尋結果</returns>
        public virtual IQueryable<TEntity> Search<TEntity>(
            string likePatten)
            where TEntity : class {
            return SearchAsync<TEntity>(likePatten).ToSync();
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <param name="properties">搜尋屬性名稱</param>
        /// <returns>搜尋結果</returns>
        public virtual async Task<IQueryable<TEntity>> SearchAsync<TEntity>(
            string likePatten,
            params string[] properties)
            where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic.SearchAsync(likePatten, properties));
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <param name="properties">搜尋屬性名稱</param>
        /// <returns>搜尋結果</returns>
        public virtual IQueryable<TEntity> Search<TEntity>(
            string likePatten,
            params string[] properties)
            where TEntity : class {
            return SearchAsync<TEntity>(likePatten, properties).ToSync();
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <param name="propertySelectors">比對屬性選擇器</param>
        /// <returns>搜尋結果</returns>
        public virtual async Task<IQueryable<TEntity>> SearchAsync<TEntity>(
            string likePatten,
            params Expression<Func<TEntity, object>>[] propertySelectors)
            where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic.SearchAsync(likePatten, propertySelectors));
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <param name="propertySelectors">比對屬性選擇器</param>
        /// <returns>搜尋結果</returns>
        public virtual IQueryable<TEntity> Search<TEntity>(
            string likePatten,
            params Expression<Func<TEntity, object>>[] propertySelectors)
            where TEntity : class {
            return SearchAsync<TEntity>(likePatten, propertySelectors).ToSync();
        }

        /// <summary>
        /// 檢查是否存在指定實例
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="id">唯一識別號</param>
        /// <returns>是否存在實例</returns>
        public virtual async Task<bool> ExistsAsync<TEntity>(object id) where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic.ExistsAsync(id));
        }

        /// <summary>
        /// 檢查是否存在指定實例
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="id">唯一識別號</param>
        /// <returns>是否存在實例</returns>
        public virtual bool Exists<TEntity>(object id) where TEntity : class {
            return ExistsAsync<TEntity>(id).ToSync();
        }

        /// <summary>
        /// 尋找一個項目
        /// </summary>
        /// <param name="cond">條件</param>
        /// <returns>尋找目標</returns>
        public virtual async Task<TEntity> FindOneAsync<TEntity>(
            Expression<Func<TEntity, bool>> cond = null)
             where TEntity : class {
            if (cond == null) cond = x => true;
            return (await ListAsync<TEntity>(cond)).SingleOrDefault();
        }

        /// <summary>
        /// 尋找一個項目
        /// </summary>
        /// <param name="cond">條件</param>
        /// <returns>尋找目標</returns>
        public virtual TEntity FindOne<TEntity>(Expression<Func<TEntity, bool>> cond = null)
            where TEntity : class {
            return FindOneAsync<TEntity>(cond).ToSync();
        }

        /// <summary>
        /// 透過唯一識別號取得指定物件實例
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>物件實例</returns>
        public async Task<TEntity> GetAsync<TEntity>(object id, TParameters parameters = default(TParameters)) where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic).GetAsync(id, parameters);
        }

        /// <summary>
        /// 透過唯一識別號取得指定物件實例
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>物件實例</returns>
        public TEntity Get<TEntity>(object id, TParameters parameters = default(TParameters)) where TEntity : class {
            return GetAsync<TEntity>(id, parameters).ToSync();
        }

        /// <summary>
        /// 透過唯一識別號取得指定物件實例
        /// </summary>
        /// <param name="type">實例類型</param>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>物件實例</returns>
        public async Task<object> GetAsync(Type type, object id, TParameters parameters = default(TParameters)) {
            var targetLogic = (dynamic)GetLogicByType(type);

            return await ((dynamic)targetLogic).GetAsync(id, parameters);
        }

        /// <summary>
        /// 透過唯一識別號取得指定物件實例
        /// </summary>
        /// <param name="type">實例類型</param>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>物件實例</returns>
        public object Get(Type type, object id, TParameters parameters = default(TParameters)) {
            return GetAsync(type, id, parameters).ToSync();
        }

        /// <summary>
        /// 加入新的物件實例
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>加入後的物件</returns>
        public async Task<TEntity> CreateAsync<TEntity>(TEntity entity, TParameters parameters = default(TParameters)) where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic).CreateAsync(entity, parameters);
        }

        /// <summary>
        /// 加入新的物件實例
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>加入後的物件</returns>
        public TEntity Create<TEntity>(TEntity entity, TParameters parameters = default(TParameters)) where TEntity : class {
            return CreateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 更新指定的物件實例
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>加入後的物件</returns>
        public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity, TParameters parameters = default(TParameters)) where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic).UpdateAsync(entity, parameters);
        }

        /// <summary>
        /// 更新指定的物件實例
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>加入後的物件</returns>
        public TEntity Update<TEntity>(TEntity entity, TParameters parameters = default(TParameters)) where TEntity : class {
            return UpdateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 更新或建立指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public async Task<TEntity> UpdateOrCreateAsync<TEntity>(TEntity entity, TParameters parameters = default(TParameters))
            where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await ((dynamic)targetLogic).UpdateOrCreateAsync(entity, parameters);
        }

        /// <summary>
        /// 更新或建立指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public TEntity UpdateOrCreate<TEntity>(TEntity entity, TParameters parameters = default(TParameters))
            where TEntity : class {
            return UpdateOrCreateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 刪除指定的物件
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        public async Task DeleteAsync<TEntity>(object id, TParameters parameters = default(TParameters)) where TEntity : class {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            await targetLogic.DeleteAsync(id, parameters);
        }

        /// <summary>
        /// 刪除指定的物件
        /// </summary>
        /// <typeparam name="T">實例類型</typeparam>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        public void Delete<TEntity>(object id, TParameters parameters = default(TParameters)) where TEntity : class {
            DeleteAsync<TEntity>(id, parameters).ToSync();
        }

        /// <summary>
        /// 刪除指定的物件
        /// </summary>
        /// <param name="type">實例類型</param>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        public async Task DeleteAsync(Type type, object id, TParameters parameters = default(TParameters)) {
            var targetLogic = (dynamic)GetLogicByType(type);

            await targetLogic.DeleteAsync(id, parameters);
        }

        /// <summary>
        /// 刪除指定的物件
        /// </summary>
        /// <param name="type">實例類型</param>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        public void Delete(Type type, object id, TParameters parameters = default(TParameters)) {
            DeleteAsync(type, id, parameters).ToSync();
        }

        /// <summary>
        /// 取得指定實例唯一識別號值
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <returns>唯一識別號</returns>
        public async Task<object> GetEntityIdentity(object entity) {
            var entryType = entity.GetType();
            dynamic logic = GetLogicByType(entryType);
            return entryType.GetProperty((string)logic.IdentityPropertyName).GetValue(entity);
        }

        /// <summary>
        /// 取得指定實例唯一識別號屬性
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <returns>唯一識別號屬性</returns>
        public async Task<PropertyInfo> GetEntityIdentityProperty(object entity) {
            var entryType = entity.GetType();
            dynamic logic = GetLogicByType(entryType);
            return entryType.GetProperty((string)logic.IdentityPropertyName);
        }

        /// <summary>
        /// 取得更新物件與資料庫內物件的資料差異
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <returns>物件差異集合</returns>
        public virtual async Task<ICollection<Difference>> GetDifferencesAsync<TEntity>(TEntity entity) {
            var targetLogic = (dynamic)GetLogicByType(typeof(TEntity));

            return await targetLogic.GetDifferencesAsync(entity);
        }

        /// <summary>
        /// 取得更新物件與資料庫內物件的資料差異
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <returns>物件差異集合</returns>
        public virtual ICollection<Difference> GetDifferences<TEntity>(TEntity entity) {
            return GetDifferencesAsync(entity).ToSync();
        }

        /// <summary>
        /// 取得直接關聯物件鏈中所有物件包含自身，如類型A有一對多的B類型屬性，則使用其中B類型物鍵取得直接關聯物件鏈則為[B,A]
        /// </summary>
        /// <typeparam name="TEntity">實例類型</typeparam>
        /// <param name="id">物件唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>直接關聯物件鏈物件陣列</returns>
        public async Task<object[]> GetDirectChain<TEntity>(object id, TParameters parameters = default(TParameters))
            where TEntity : class {
            return await GetDirectChain(typeof(TEntity), await GetAsync<TEntity>(id, parameters), parameters);
        }

        /// <summary>
        /// 取得直接關聯物件鏈中所有物件包含自身，如類型A有一對多的B類型屬性，則使用其中B類型物鍵取得直接關聯物件鏈則為[B,A]
        /// </summary>
        /// <param name="type">實例類型</param>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>直接關聯物件鏈物件陣列</returns>
        public async Task<object[]> GetDirectChain(Type type, object entity, TParameters parameters = default(TParameters)) {
            var logic = (dynamic)GetLogicByType(type);
            return await logic.GetDirectChain(entity, parameters);
        }

        #region GlobalHook
        /// <summary>
        /// 建立前處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task BeforeCreate(object entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 建立後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task BeforeUpdate(object entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 刪除前處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task BeforeDelete(object entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 取得後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task AfterGet(object entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 建立後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task AfterCreate(object entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 更新後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task AfterUpdate(object entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 刪除後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task AfterDelete(object entity, TParameters parameters = default(TParameters)) { }
        #endregion
    }
}