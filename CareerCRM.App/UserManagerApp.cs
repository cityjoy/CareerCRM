﻿using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using CareerCRM.App.Interface;
using CareerCRM.App.Request;
using CareerCRM.App.Response;
using CareerCRM.Repository.Domain;
using CareerCRM.Repository.Interface;


namespace CareerCRM.App
{
    public class UserManagerApp : BaseApp<User>
    {
        private RevelanceManagerApp _revelanceApp;

        private IAuth _auth;

        public User GetByAccount(string account)
        {
            return Repository.FindSingle(u => u.Account == account);
        }

        /// <summary>
        /// 加载当前登录用户可访问的一个部门及子部门全部用户
        /// </summary>
        public TableData Load(QueryUserListReq request)
        {
            var loginUser = _auth.GetCurrentUser();

            string cascadeId = ".0.";
            if (!string.IsNullOrEmpty(request.orgId))
            {
                var org = loginUser.Orgs.SingleOrDefault(u => u.Id == request.orgId);
                cascadeId = org.CascadeId;
            }

            IQueryable<User> query = UnitWork.Find<User>(null);
            if (!string.IsNullOrEmpty(request.key))
            {
                query = UnitWork.Find<User>(u => u.Name.Contains(request.key) || u.Account.Contains(request.key));
            }

            var ids = loginUser.Orgs.Where(u => u.CascadeId.Contains(cascadeId)).Select(u => u.Id).ToArray();
            var userIds = _revelanceApp.Get(Define.USERORG, false, ids);

            var users = query.Where(u => userIds.Contains(u.Id))
                   .OrderBy(u => u.Name)
                   .Skip((request.page - 1) * request.limit)
                   .Take(request.limit)
                   .ToList();

            var records = query.Count(u => userIds.Contains(u.Id));


            var userviews = new List<UserView>();
            foreach (var user in users)
            {
                UserView uv = AutoMapperExt.MapTo<UserView>(user);
                var orgs = LoadByUser(user.Id);
                uv.Organizations = string.Join(",", orgs.Select(u => u.Name).ToList());
                uv.OrganizationIds = string.Join(",", orgs.Select(u => u.Id).ToList());
                userviews.Add(uv);
            }

            return new TableData
            {
                count = records,
                data = userviews,
            };
        }

        public void AddOrUpdate(UserView view)
        {
            if (string.IsNullOrEmpty(view.OrganizationIds))
                throw new Exception("请为用户分配机构");
            User user = view;
            if (string.IsNullOrEmpty(view.Id))
            {
                if (Repository.IsExist(u => u.Account == view.Account))
                {
                    throw new Exception("用户账号已存在");
                }
                if (string.IsNullOrWhiteSpace(user.Password)||user.Password.Length < 6)
                {
                    throw new Exception("密码长度要大于6");

                }
                user.Password = StringExtensions.ToMd5(user.Password); //加密密码
                user.CreateTime = DateTime.Now;
                Repository.Add(user);
                view.Id = user.Id;   //要把保存后的ID存入view
            }
            else
            {
                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    Repository.Update(u => u.Id == view.Id, u => new User
                    {
                        BizCode = user.BizCode,
                        Name = user.Name,
                        Sex = user.Sex,
                        Status = user.Status,
                        IsMaster = user.IsMaster

                    });
                }
               
                else
                {
                    if (user.Password.Length > 0 && user.Password.Length < 6)
                    {
                        throw new Exception("密码长度要大于6");

                    }
                    Repository.Update(u => u.Id == view.Id, u => new User
                    {
                        BizCode = user.BizCode,
                        Password = StringExtensions.ToMd5(user.Password), //加密密码
                        Name = user.Name,
                        Sex = user.Sex,
                        Status = user.Status,
                        IsMaster = user.IsMaster

                    });
                }
            }
            UnitWork.Save();
            string[] orgIds = view.OrganizationIds.Split(',').ToArray();

            _revelanceApp.DeleteBy(Define.USERORG, user.Id);
            _revelanceApp.Assign(Define.USERORG, orgIds.ToLookup(u => user.Id));
        }

        /// <summary>
        /// 加载用户的所有机构
        /// </summary>
        public IEnumerable<Org> LoadByUser(string userId)
        {
            var result = from userorg in UnitWork.Find<Relevance>(null)
                         join org in UnitWork.Find<Org>(null) on userorg.SecondId equals org.Id
                         where userorg.FirstId == userId && userorg.Key == Define.USERORG
                         select org;
            return result;
        }


        public UserManagerApp(IUnitWork unitWork, IRepository<User> repository,
            RevelanceManagerApp app, IAuth auth) : base(unitWork, repository)
        {
            _revelanceApp = app;
            _auth = auth;
        }
    }
}