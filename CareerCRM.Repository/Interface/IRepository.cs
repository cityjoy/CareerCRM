using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CareerCRM.Repository.Interface
{
    public interface IRepository<T> where T : class
    {
        T FindSingle(Expression<Func<T, bool>> exp = null);
        bool IsExist(Expression<Func<T, bool>> exp);
        IQueryable<T> Find(Expression<Func<T, bool>> exp = null);

        IQueryable<T> Find(int pageindex = 1, int pagesize = 10, string orderby = "",
            Expression<Func<T, bool>> exp = null);
        IQueryable<T> GetPageList<Tkey>(int pageSize, int pageIndex, out int total,
            Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderbyPredicate, bool isAsc);
        int GetCount(Expression<Func<T, bool>> exp = null);

        void Add(T entity, bool isSaved = true);

        void BatchAdd(ICollection<T> entities, bool isSaved=true);

        /// <summary>
        /// 更新一个实体的所有属性
        /// </summary>
        void Update(T entity, bool isSaved = true);

        void Delete(T entity, bool isSaved = true);
        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update(u =>u.Id==1,u =>new User{Name="ok"});</para>
        /// </summary>
        /// <param name="where">更新条件</param>
        /// <param name="entity">更新后的实体</param>
        bool Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);
        /// <summary>
        /// 批量删除
        /// </summary>
        bool Delete(Expression<Func<T, bool>> exp);
        int ExecuteSql(string sql);
    }
}