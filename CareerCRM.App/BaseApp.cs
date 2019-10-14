using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CareerCRM.Repository.Core;
using CareerCRM.Repository.Domain;
using CareerCRM.Repository.Interface;

namespace CareerCRM.App
{
    /// <summary>
    /// 业务层基类，UnitWork用于事务操作，Repository用于普通的数据库操作
    /// <para>如用户管理：Class UserManagerApp:BaseApp<User></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseApp<T> where T:Entity
    {
        /// <summary>
        /// 用于事务操作
        /// </summary>
        /// <value>The unit work.</value>
        protected IUnitWork UnitWork;

        public BaseApp(IUnitWork unitWork, IRepository<T> repository)
        {
            UnitWork = unitWork;
            this.Repository = repository;
        }

        /// <summary>
        /// 用于普通的数据库操作
        /// </summary>
        /// <value>The Repository.</value>
        protected IRepository<T> Repository;

        #region  CRUD 操作
        /// <summary>
        /// 通过Lamda表达式获取实体列表
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> result = null;
            result = Repository.Find(predicate);
            return result;
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
        public virtual IQueryable<T> GetPageList<Tkey>(int pageSize, int pageIndex, out int total, Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderbyPredicate, bool isAsc)
        {
            total = 0;
            IQueryable<T> result = null;
            result = Repository.GetPageList(pageSize, pageIndex, out total, predicate, orderbyPredicate, isAsc);
            return result;
        }

        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public virtual T GetSingle(Expression<Func<T, bool>> predicate = null)
        {
            T rsult = null;
            rsult = Repository.FindSingle(predicate);

            return rsult;

        }

        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        public virtual bool Add(T entity, bool isSave = true)
        {
            bool result = false;
            Repository.Add(entity);
            result = isSave ? UnitWork.Save() : false;
            return result;
        }
        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <summary>
        public virtual void BulkInsert(IList<T> entities)
        {

            Repository.BatchAdd(entities);
        }
        /// <summary>
        /// 根据条件更新记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        public virtual bool Update(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T>> updateExpression = null, bool isSave = true)
        {
            bool result = false;
            result = Repository.Update(predicate, updateExpression);
            return result;
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="isSave"> </param>
        /// <returns></returns>
        public virtual bool Update(T entity, bool isSave = true)
        {
            bool result = false;

            Repository.Update(entity);
            result = isSave ? UnitWork.Save() : false;
            return result;
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="isSave"> </param>
        /// <returns></returns>
        public virtual bool Delete(T entity, bool isSave = true)
        {
            bool result = false;

            Repository.Delete(entity);
            result = isSave ? UnitWork.Save() : false;
            return result;
        }

        /// <summary>
        ///删除符合条件记录（直接commit）
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <returns></returns>
        public virtual bool Delete(Expression<Func<T, bool>> predicate = null)
        {
            bool result = false;

            result = Repository.Delete(predicate);
            return result;
        }

        /// <summary>
        /// 使用sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual int ExecuteSqlWithNonQuery(string sql)
        {
            int result = 0;

            result = Repository.ExecuteSql(sql);
            return result;
        }
         
        #endregion

        /// <summary>
        /// 按id批量删除
        /// </summary>
        /// <param name="ids"></param>
        public virtual void Delete(string[] ids)
        {
            Repository.Delete(u => ids.Contains(u.Id));
        }

        public virtual T Get(string id)
        {
            return Repository.FindSingle(u => u.Id == id);
        }

        /// <summary>
        /// 如果一个类有层级结构（树状），则修改该节点时，要修改该节点的所有子节点
        /// //修改对象的级联ID，生成类似XXX.XXX.X.XX
        /// </summary>
        /// <typeparam name="U">U必须是一个继承TreeEntity的结构</typeparam>
        /// <param name="entity"></param>

        public virtual void ChangeModuleCascade<U>(U entity) where U:TreeEntity
        {
            string cascadeId;
            int currentCascadeId = 1;  //当前结点的级联节点最后一位
            var sameLevels = UnitWork.Find<U>(o => o.ParentId == entity.ParentId && o.Id != entity.Id);
            foreach (var obj in sameLevels)
            {
                int objCascadeId = int.Parse(obj.CascadeId.TrimEnd('.').Split('.').Last());
                if (currentCascadeId <= objCascadeId) currentCascadeId = objCascadeId + 1;
            }

            if (!string.IsNullOrEmpty(entity.ParentId))
            {
                var parentOrg = UnitWork.FindSingle<U>(o => o.Id == entity.ParentId);
                if (parentOrg != null)
                {
                    cascadeId = parentOrg.CascadeId + currentCascadeId + ".";
                    entity.ParentName = parentOrg.Name;
                }
                else
                {
                    throw new Exception("未能找到该组织的父节点信息");
                }
            }
            else
            {
                cascadeId = ".0." + currentCascadeId + ".";
                entity.ParentName = "根节点";
            }

            entity.CascadeId = cascadeId;
        }
    }
}
