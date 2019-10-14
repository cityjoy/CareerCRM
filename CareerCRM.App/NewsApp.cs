

using AutoMapper;
using AutoMapper.QueryableExtensions;
using CareerCRM.App.Interface;
using CareerCRM.App.Request;
using CareerCRM.App.Response;
using CareerCRM.App.SSO;
using CareerCRM.Repository.Domain;
using CareerCRM.Repository.Interface;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace CareerCRM.App
{
    public class NewsApp : BaseApp<News>
    {
        private IAuth _auth;

        public NewsApp(IUnitWork unitWork, IRepository<News>
            repository, IAuth auth) : base(unitWork, repository)
        {
            _auth = auth;
        }
        /// <summary>
        /// 加载列表
        ///</summary>
        public TableData Load(NewsListReq request)
        {
            var loginUser = _auth.GetCurrentUser();
            Expression<Func<News, bool>> exp = u => true;

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                exp = exp.And(m => m.Name.Contains(request.Name));
            }

            if (DateTime.MinValue != (request.StartCreateTime) && DateTime.MinValue != request.EndCreateTime)
            {
                exp = exp.And(m => m.CreateTime > request.StartCreateTime && m.CreateTime < request.EndCreateTime);
            }
            else if (DateTime.MinValue != (request.StartCreateTime))
            {
                exp = exp.And(m => m.CreateTime > request.StartCreateTime);
            }
            else if (DateTime.MinValue != (request.EndCreateTime))
            {
                exp = exp.And(m => m.CreateTime < request.EndCreateTime);
            }

            if (null != request.IsPublish)
            {
                exp = exp.And(m => m.IsPublish == request.IsPublish.Value);
            }
            #region 使用AutoMapper获取List视图数据

            var configuration = new MapperConfiguration(cfg => cfg.CreateMap<News, NewsListVM>());
            var NewsList = Repository.Find(request.page, 10, "", exp).ProjectTo<NewsListVM>(configuration).ToList();
            #endregion
            #region 手动编码获取List视图数据
            //var NewsList = Repository.Find(request.page, 10)
            // .Select(m => new  NewsListVM{
            // Name  = m.Name ,// CreateTime  = m.CreateTime ,// IsPublish  = m.IsPublish ,// Id  = m.Id  
            //})
            //.ToList< NewsListVM>();
            #endregion
            var records = Repository.GetCount(exp);
            return new TableData
            {
                count = records,
                data = NewsList,
            };
        }
        /// <summary>
        /// 更新
        ///</summary>
        /// <param name="obj"></param>
        public void Update(NewsFormDTO obj)
        {
            Repository.Update(u => u.Id == obj.Id, u => new News
            {

                Name = obj.Name,
                Content = obj.Content,
                IsPublish = obj.IsPublish,
            });

        }

    }
}


