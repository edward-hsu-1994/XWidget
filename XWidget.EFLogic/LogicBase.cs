using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Dynamic.Core;
using XWidget.Web.Exceptions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections;
using AutoCompare;

namespace XWidget.EFLogic {
    /// <summary>
    /// 基礎邏輯操作器基礎
    /// </summary>
    public abstract class LogicBase<TContext, TEntity, TId>
        : LogicBase<TContext, TEntity, TId, object[]>
        where TEntity : class
        where TContext : DbContext {

        /// <summary>
        /// 基礎邏輯操作器基礎建構子，預設的主鍵為Id
        /// </summary>
        /// <param name="logicManager">資料庫操作邏輯管理器實例</param>
        public LogicBase(LogicManagerBase<TContext, object[]> logicManager) : base(logicManager, "Id") {
        }

        /// <summary>
        /// 基礎邏輯操作器基礎建構子
        /// </summary>
        /// <param name="logicManager">資料庫操作邏輯管理器實例</param>
        /// <param name="identityPropertyName">唯一識別號屬性選擇器</param>
        public LogicBase(LogicManagerBase<TContext, object[]> logicManager, string identityPropertyName) : base(logicManager, identityPropertyName) {
        }

        /// <summary>
        /// 基礎邏輯操作器基礎建構子
        /// </summary>
        /// <param name="logicManager">資料庫操作邏輯管理器實例</param>
        /// <param name="identitySelector">唯一識別號屬性選擇器</param>
        public LogicBase(LogicManagerBase<TContext, object[]> logicManager, Expression<Func<TEntity, TId>> identitySelector) : base(logicManager, identitySelector) {
        }
    }

    /// <summary>
    /// 基礎邏輯操作器基礎
    /// </summary>
    public abstract class LogicBase<TContext, TEntity, TId, TParameters>
    where TEntity : class
    where TContext : DbContext {
        /// <summary>
        /// 資料庫操作邏輯管理器
        /// </summary>
        public LogicManagerBase<TContext, TParameters> Manager { get; private set; }

        /// <summary>
        /// 資料庫內容實例
        /// </summary>
        public TContext Database => Manager.Database;

        /// <summary>
        /// 唯一識別號屬性名稱
        /// </summary>
        public string IdentityPropertyName { get; private set; }

        /// <summary>
        /// 建立實例時略過唯一識別號欄位
        /// </summary>
        public bool CreateIgnoreIdentity { get; set; } = true;


        /// <summary>
        /// 基礎邏輯操作器基礎建構子，預設的主鍵為Id
        /// </summary>
        /// <param name="logicManager">資料庫操作邏輯管理器實例</param>
        public LogicBase(LogicManagerBase<TContext, TParameters> logicManager) : this(logicManager, "Id") {

        }

        /// <summary>
        /// 基礎邏輯操作器基礎建構子
        /// </summary>
        /// <param name="logicManager">資料庫操作邏輯管理器實例</param>
        /// <param name="identityPropertyName">唯一識別號屬性選擇器</param>
        public LogicBase(LogicManagerBase<TContext, TParameters> logicManager, string identityPropertyName) {
            this.Manager = logicManager;


            var idProperties = typeof(TEntity).GetProperties().Where(x => x.PropertyType == typeof(TId));

            if (idProperties.Count() == 0) {
                throw new InvalidOperationException("在指定類型中找不到指定的主鍵類型");
            }

            var idProperty = idProperties.FirstOrDefault(x => x.Name.ToLower() == identityPropertyName);

            if (idProperty == null) {
                idProperty = idProperties.FirstOrDefault(x => x.Name.ToLower() == (typeof(TEntity).Name + identityPropertyName).ToLower());
            }

            if (idProperty == null && idProperties.Count() == 1) {
                idProperty = idProperties.First();
            }

            if (idProperties == null) {
                throw new InvalidOperationException("在指定類型中找不到名稱為Id的屬性作為預設主鍵");
            }

            this.IdentityPropertyName = identityPropertyName;
        }

        /// <summary>
        /// 基礎邏輯操作器基礎建構子
        /// </summary>
        /// <param name="logicManager">資料庫操作邏輯管理器實例</param>
        /// <param name="identitySelector">唯一識別號屬性選擇器</param>
        public LogicBase(LogicManagerBase<TContext, TParameters> logicManager, Expression<Func<TEntity, TId>> identitySelector) {
            this.Manager = logicManager;
            this.IdentityPropertyName = (identitySelector.Body as MemberExpression).Member.Name;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="cond">條件</param>
        /// <returns>列表</returns>
        public virtual async Task<IQueryable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> cond = null) {
            if (cond == null) cond = x => true;
            return Database.Set<TEntity>().Where(cond);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="cond">條件</param>
        /// <returns>列表</returns>
        public IQueryable<TEntity> List(Expression<Func<TEntity, bool>> cond = null) {
            return ListAsync(cond).ToSync();
        }

        /// <summary>
        /// 全文搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <returns>搜尋結果</returns>
        public virtual async Task<IQueryable<TEntity>> SearchAsync(
            string likePatten) {
            return await SearchAsync(likePatten, new Expression<Func<TEntity, object>>[] { });
        }

        /// <summary>
        /// 全文搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <returns>搜尋結果</returns>
        public IQueryable<TEntity> Search(
            string likePatten) {
            return SearchAsync(likePatten).ToSync();
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <param name="properties">搜尋屬性名稱</param>
        /// <returns>搜尋結果</returns>
        public virtual async Task<IQueryable<TEntity>> SearchAsync(
            string likePatten,
            params string[] properties) {
            return await SearchAsync(likePatten, properties.Select(x => {
                var p = Expression.Parameter(typeof(TEntity), "x");

                return Expression.Lambda<Func<TEntity, object>>(
                    Expression.MakeMemberAccess(p, typeof(TEntity).GetMember(x).First()), p
                );
            }).ToArray());
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <param name="properties">搜尋屬性名稱</param>
        /// <returns>搜尋結果</returns>
        public IQueryable<TEntity> Search(
            string likePatten,
            params string[] properties) {
            return SearchAsync(likePatten, properties).ToSync();
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <param name="propertySelectors">比對屬性選擇器</param>
        /// <returns>搜尋結果</returns>
        public virtual async Task<IQueryable<TEntity>> SearchAsync(
            string likePatten,
            params Expression<Func<TEntity, object>>[] propertySelectors) {
            if (propertySelectors.Length == 0) {
                var clrType = Database.Model.FindEntityType(typeof(TEntity));
                var properties = clrType.GetProperties()
                    .Where(x => x.PropertyInfo.PropertyType == typeof(string))
                    .Select(x => x.PropertyInfo.Name).ToArray();

                if (properties.Length == 0) { //防止Loop
                    return await this.ListAsync(x => false);
                }

                return await SearchAsync(likePatten, properties);
            }

            var p = Expression.Parameter(typeof(TEntity), "x");

            var likeMethod = typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), typeof(string), typeof(string) });

            List<Expression> equalExpList = new List<Expression>();

            // OR串接
            Expression AllOr(IEnumerable<Expression> exps) {
                Expression result = exps.First();

                foreach (var exp in exps.Skip(1)) {
                    result = Expression.OrElse(result, exp);
                }
                return result;
            }

            var EFFunctionsMember = typeof(EF).GetProperty("Functions");

            MemberExpression ReplaceParameter(MemberExpression expression) {
                if (expression.Expression is ParameterExpression exp) {
                    return Expression.MakeMemberAccess(p, expression.Member);
                } else if (expression.Expression is MemberExpression exm) {
                    return Expression.MakeMemberAccess(ReplaceParameter(exm), expression.Member);
                } else {
                    return expression;
                }
            }

            propertySelectors //轉換MemberEXpression為Like呼叫
                .Select(x => {
                    equalExpList.Add(
                        Expression.Call(
                            likeMethod,
                            new Expression[]{
                                Expression.MakeMemberAccess(null,EFFunctionsMember),
                                ReplaceParameter((MemberExpression)x.Body),
                                Expression.Constant(likePatten)
                            }
                        )
                    );
                    return x;
                }).ToArray();

            var queryExpression = Expression.Lambda<Func<TEntity, bool>>(
                AllOr(equalExpList), p
            );
            return (IQueryable<TEntity>)Queryable.Where(Database.Set<TEntity>(), queryExpression);
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="likePatten">SQL Like模式</param>
        /// <param name="propertySelectors">比對屬性選擇器</param>
        /// <returns>搜尋結果</returns>
        public IQueryable<TEntity> Search(
            string likePatten,
            params Expression<Func<TEntity, object>>[] propertySelectors) {
            return SearchAsync(likePatten, propertySelectors).ToSync();
        }

        /// <summary>
        /// 檢查是否存在指定實例
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <returns>是否存在實例</returns>
        public virtual async Task<bool> ExistsAsync(object id) {
            return Database.Set<TEntity>().Any($"{IdentityPropertyName} == @0", id);
        }

        /// <summary>
        /// 檢查是否存在指定實例
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <returns>是否存在實例</returns>
        public bool Exists(object id) {
            return ExistsAsync(id).ToSync();
        }

        /// <summary>
        /// 檢查是否存在指定實例
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <returns>是否存在實例</returns>
        public virtual async Task<bool> ExistsAsync(TId id) {
            return Database.Set<TEntity>().Any($"{IdentityPropertyName} == @0", id);
        }

        /// <summary>
        /// 檢查是否存在指定實例
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <returns>是否存在實例</returns>
        public bool Exists(TId id) {
            return ExistsAsync(id).ToSync();
        }

        /// <summary>
        /// 尋找一個項目
        /// </summary>
        /// <param name="cond">條件</param>
        /// <returns>尋找目標</returns>
        public virtual async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> cond = null) {
            if (cond == null) cond = x => true;
            return (await ListAsync(cond)).SingleOrDefault();
        }

        /// <summary>
        /// 尋找一個項目
        /// </summary>
        /// <param name="cond">條件</param>
        /// <returns>尋找目標</returns>
        public TEntity FindOne(Expression<Func<TEntity, bool>> cond = null) {
            return FindOneAsync(cond).ToSync();
        }

        /// <summary>
        /// 透過唯一識別號取得指定物件實例
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>物件實例</returns>
        public async Task<TEntity> GetAsync(object id, TParameters parameters = default(TParameters)) {
            return await GetAsync((TId)id, parameters);
        }

        /// <summary>
        /// 透過唯一識別號取得指定物件實例
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>物件實例</returns>
        public TEntity Get(object id, TParameters parameters = default(TParameters)) {
            return GetAsync(id, parameters).ToSync();
        }

        /// <summary>
        /// 透過唯一識別號取得指定物件實例
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>物件實例</returns>
        public virtual async Task<TEntity> GetAsync(TId id, TParameters parameters = default(TParameters)) {
            var instance = Database.Set<TEntity>().SingleOrDefault($"{IdentityPropertyName} == @0", id);

            if (instance == null) {
                throw new NotFoundException();
            }

            await Manager.AfterGet(instance, parameters);
            await AfterGet(instance, parameters);
            return instance;
        }

        /// <summary>
        /// 透過唯一識別號取得指定物件實例
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        /// <returns>物件實例</returns>
        public TEntity Get(TId id, TParameters parameters = default(TParameters)) {
            return GetAsync(id, parameters).ToSync();
        }

        /// <summary>
        /// 加入新的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>加入後的物件</returns>
        public async Task<TEntity> CreateAsync(object entity, TParameters parameters = default(TParameters)) {
            return await CreateAsync((TEntity)entity, parameters);
        }

        /// <summary>
        /// 加入新的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>加入後的物件</returns>
        public TEntity Create(object entity, TParameters parameters = default(TParameters)) {
            return CreateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 加入新的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>加入後的物件</returns>
        public virtual async Task<TEntity> CreateAsync(TEntity entity, TParameters parameters = default(TParameters)) {
            if (CreateIgnoreIdentity) {
                (await Manager.GetEntityIdentityProperty(entity)).SetValue(entity, default(TId));
            }

            Database.Add(entity);

            await Manager.BeforeCreate(entity, parameters);
            await BeforeCreate(entity, parameters);
            await Database.SaveChangesAsync();
            await AfterCreate(entity, parameters);
            await Manager.AfterCreate(entity, parameters);

            var type = typeof(TEntity);
            TId id = (TId)type.GetProperty(IdentityPropertyName).GetValue(entity);
            return await GetAsync(id, parameters);
        }

        /// <summary>
        /// 加入新的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>加入後的物件</returns>
        public TEntity Create(TEntity entity, TParameters parameters = default(TParameters)) {
            return CreateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 更新指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public async Task<TEntity> UpdateAsync(object entity, TParameters parameters = default(TParameters)) {
            return await UpdateAsync((TEntity)entity, parameters);
        }

        /// <summary>
        /// 更新指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public TEntity Update(object entity, TParameters parameters = default(TParameters)) {
            return UpdateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 更新或建立指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public virtual async Task<TEntity> UpdateOrCreateAsync(object entity, TParameters parameters = default(TParameters)) {
            var type = typeof(TEntity);
            TId id = (TId)type.GetProperty(IdentityPropertyName).GetValue(entity);

            if (await ExistsAsync(id)) {
                return await UpdateAsync((TEntity)entity, parameters);
            } else {
                return await CreateAsync((TEntity)entity, parameters);
            }
        }

        /// <summary>
        /// 更新或建立指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public TEntity UpdateOrCreate(object entity, TParameters parameters = default(TParameters)) {
            return UpdateOrCreateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 更新指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, TParameters parameters = default(TParameters)) {
            return await InternalUpdateAsync(entity, new List<object>(), parameters);
        }

        /// <summary>
        /// 更新指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        private async Task<TEntity> InternalUpdateAsync(TEntity entity, List<object> refList, TParameters parameters = default(TParameters)) {
            var type = typeof(TEntity);
            TId id = (TId)type.GetProperty(IdentityPropertyName).GetValue(entity);
            var instance = await GetAsync(id, parameters);

            foreach (var member in Database.Entry(instance).Members) {
                var obj = member.Metadata.PropertyInfo.GetValue(entity);

                if (obj != null) {
                    if (!obj.GetType().IsValueType && refList.Contains(obj)) { // 防止循環參照
                        continue;
                    }

                    refList.Add(obj);
                }

                if (member is ReferenceEntry) {
                    if (obj == null) continue;

                    if (Database.Model.FindEntityType(obj.GetType()) == null) continue;

                    ((dynamic)Manager.GetLogicByType(member.Metadata.PropertyInfo.PropertyType)).UpdateOrCreate(obj);
                } else if (member is CollectionEntry) {
                    var collection = (IEnumerable)obj;

                    if (obj == null) continue;

                    foreach (var item in collection) {
                        if (item == null) continue;
                        if (Database.Model.FindEntityType(item.GetType()) == null) continue;
                        ((dynamic)Manager.GetLogicByType(item.GetType())).UpdateOrCreate(item);
                    }
                } else {
                    member.Metadata.PropertyInfo.SetValue(
                        instance,
                        member.Metadata.PropertyInfo.GetValue(entity));
                }
            }

            await Manager.BeforeUpdate(instance, parameters);
            await BeforeUpdate(instance, parameters);
            Database.Update(instance);
            await Database.SaveChangesAsync();
            await AfterUpdate(instance, parameters);
            await Manager.AfterUpdate(instance, parameters);
            return instance;
        }

        /// <summary>
        /// 更新指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public TEntity Update(TEntity entity, TParameters parameters = default(TParameters)) {
            return UpdateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 更新或建立指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public virtual async Task<TEntity> UpdateOrCreateAsync(TEntity entity, TParameters parameters = default(TParameters)) {
            var type = typeof(TEntity);
            TId id = (TId)type.GetProperty(IdentityPropertyName).GetValue(entity);

            if (await ExistsAsync(id)) {
                return await UpdateAsync(entity, parameters);
            } else {
                return await CreateAsync(entity, parameters);
            }
        }

        /// <summary>
        /// 更新或建立指定的物件實例
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>更新後的物件實例</returns>
        public TEntity UpdateOrCreate(TEntity entity, TParameters parameters = default(TParameters)) {
            return UpdateOrCreateAsync(entity, parameters).ToSync();
        }

        /// <summary>
        /// 刪除指定的物件
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        public async Task DeleteAsync(object id, TParameters parameters = default(TParameters)) {
            await DeleteAsync((TId)id, parameters);
        }

        /// <summary>
        /// 刪除指定的物件
        /// </summary>
        /// <param name="id">唯一識別號</param>
        /// <param name="parameters">參數</param>
        public void Delete(object id, TParameters parameters = default(TParameters)) {
            DeleteAsync(id, parameters).ToSync();
        }

        /// <summary>
        /// 刪除指定的物件
        /// </summary>
        /// <param name="id">物件唯一識別號</param>
        /// <param name="parameters">參數</param>
        public virtual async Task DeleteAsync(TId id, TParameters parameters = default(TParameters)) {
            var instance = await GetAsync(id, parameters);

            if (instance == null) {
                throw new NotFoundException();
            }

            Database.RemoveCascade(instance);

            await Manager.BeforeDelete(instance, parameters);
            await BeforeDelete(instance, parameters);
            await Database.SaveChangesAsync();
            await AfterDelete(instance, parameters);
            await Manager.AfterDelete(instance, parameters);
        }


        /// <summary>
        /// 刪除指定的物件
        /// </summary>
        /// <param name="id">物件唯一識別號</param>
        /// <param name="parameters">參數</param>
        public void Delete(TId id, TParameters parameters = default(TParameters)) {
            DeleteAsync(id, parameters).ToSync();
        }

        /// <summary>
        /// 取得更新物件與資料庫內物件的資料差異
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <returns>物件差異集合</returns>
        public virtual async Task<ICollection<Difference>> GetDifferencesAsync(TEntity entity) {
            var type = typeof(TEntity);
            TId id = (TId)type.GetProperty(IdentityPropertyName).GetValue(entity);

            var instance = Database.Set<TEntity>().AsNoTracking().SingleOrDefault($"{IdentityPropertyName} == @0", id);
            if (instance == null) {
                throw new NotFoundException();
            }

            var differences = AutoCompare.Comparer.Compare<TEntity>(instance, entity);

            return differences;
        }

        /// <summary>
        /// 取得更新物件與資料庫內物件的資料差異
        /// </summary>
        /// <param name="entity">物件實例</param>
        /// <returns>物件差異集合</returns>
        public virtual ICollection<Difference> GetDifferences(TEntity entity) {
            return GetDifferencesAsync(entity).ToSync();
        }

        #region Hook
        /// <summary>
        /// 建立前處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task BeforeCreate(TEntity entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 建立後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task BeforeUpdate(TEntity entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 刪除前處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task BeforeDelete(TEntity entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 取得後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task AfterGet(TEntity entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 建立後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task AfterCreate(TEntity entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 更新後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task AfterUpdate(TEntity entity, TParameters parameters = default(TParameters)) { }

        /// <summary>
        /// 刪除後處理
        /// </summary>
        /// <param name="entity">實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>實例</returns>
        public virtual async Task AfterDelete(TEntity entity, TParameters parameters = default(TParameters)) { }
        #endregion
    }
}
