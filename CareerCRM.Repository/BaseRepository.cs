using System;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using CareerCRM.Repository.Core;
using CareerCRM.Repository.Interface;
using Z.EntityFramework.Plus;
using System.Collections.Generic;

namespace CareerCRM.Repository
{
   public  class BaseRepository<T> :IRepository<T> where T :Entity
   {
       private OpenAuthDBContext _context;

       public BaseRepository(OpenAuthDBContext context)
       {
           _context = context;
       }


       /// <summary>
        /// 根据过滤条件，获取记录
        /// </summary>
        /// <param name="exp">The exp.</param>
        public IQueryable<T> Find(Expression<Func<T, bool>> exp = null)
        {
            return Filter(exp);
        }

        public bool IsExist(Expression<Func<T, bool>> exp)
        {
            return _context.Set<T>().Any(exp);
        }

        /// <summary>
        /// 查找单个，且不被上下文所跟踪
        /// </summary>
        public T FindSingle(Expression<Func<T, bool>> exp)
        {
            return _context.Set<T>().AsNoTracking().FirstOrDefault(exp);
        }

        /// <summary>
        /// 得到分页记录
        /// </summary>
        /// <param name="pageindex">The pageindex.</param>
        /// <param name="pagesize">The pagesize.</param>
        /// <param name="orderby">排序，格式如："Id"/"Id descending"</param>
        public IQueryable<T> Find(int pageindex, int pagesize, string orderby = "", Expression<Func<T, bool>> exp = null)
        {
            if (pageindex < 1) pageindex = 1;
            if (string.IsNullOrEmpty(orderby))
                orderby = "Id descending";

            return Filter(exp).OrderBy(orderby).Skip(pagesize * (pageindex - 1)).Take(pagesize);
        }
        /// <summary>  
        /// 分页查询 + 条件查询 + 排序  
        /// </summary>  
        /// <typeparam name="Tkey">泛型</typeparam>  
        /// <param name="pageSize">每页大小</param>  
        /// <param name="pageIndex">当前页码</param>  
        /// <param name="total">总数量</param>  
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）查询条件</param>  
        /// <param name="orderbyPredicate">Lamda表达式（p=>p.Id）排序条件</param>  
        /// <param name="isAsc">是否升序</param>  
        /// <returns>IQueryable 泛型集合</returns> 

        public IQueryable<T> GetPageList<Tkey>(int pageSize, int pageIndex, out int total,
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, Tkey>> orderbyPredicate, bool isAsc)
        {
            IQueryable<T> result = null;
            //total = Entities.Where(predicate).Count();
            if (isAsc)
            {
                var dbSet = _context.Set<T>().AsNoTracking().AsQueryable();
                total = dbSet.Where(predicate).Count();
                var temp = dbSet
                        .Where(predicate)
                        .OrderBy<T, Tkey>(orderbyPredicate)
                        .Skip(pageSize * (pageIndex - 1))
                        .Take(pageSize);
                result = temp;
                return result;
            }
            else
            {
                var dbSet = _context.Set<T>().AsNoTracking().AsQueryable();
                total = dbSet.Where(predicate).Count();
                var temp = dbSet
                   .Where(predicate)
                   .OrderByDescending<T, Tkey>(orderbyPredicate)
                   .Skip(pageSize * (pageIndex - 1))
                   .Take(pageSize);
                result = temp;

                return result;
            }
        }
        /// <summary>
        /// 根据过滤条件获取记录数
        /// </summary>
        public int GetCount(Expression<Func<T, bool>> exp = null)
        {
            return Filter(exp).Count();
        }

        public void Add(T entity,bool isSaved = true)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
            }
            _context.Set<T>().Add(entity);
            if (isSaved)
            {
                _context.SaveChanges();
            }
            _context.Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void BatchAdd(ICollection<T> entities, bool isSaved = true)
        {
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid().ToString();
            }
            _context.Set<T>().AddRange(entities);
            if (isSaved)
            {
                _context.SaveChanges();
            }

        }

        public void Update(T entity, bool isSaved = true)
        {
            var entry = this._context.Entry(entity);
            entry.State = EntityState.Modified;

            //如果数据没有发生变化
            if (!this._context.ChangeTracker.HasChanges())
            {
                return;
            }

            if (isSaved)
            {
                _context.SaveChanges();
            }

        }

        public void Delete(T entity, bool isSaved = true)
        {
            _context.Set<T>().Remove(entity);
            if (isSaved)
            {
                _context.SaveChanges();
            }

        }


        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update(u =>u.Id==1,u =>new User{Name="ok"});</para>
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="entity">The entity.</param>
        public  bool Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity)
        {
           return _context.Set<T>().Where(where).Update(entity)>0;
        }

        public  bool Delete(Expression<Func<T, bool>> exp)
        {
            return _context.Set<T>().Where(exp).Delete()>0;
        }

        private IQueryable<T> Filter(Expression<Func<T, bool>> exp)
        {
            var dbSet = _context.Set<T>().AsNoTracking().AsQueryable();
            if (exp != null)
                dbSet = dbSet.Where(exp);
            return dbSet;
        }

       public int ExecuteSql(string sql)
       {
          return  _context.Database.ExecuteSqlCommand(sql);
       }

       
    }
}
