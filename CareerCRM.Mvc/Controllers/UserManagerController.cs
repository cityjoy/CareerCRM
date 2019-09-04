﻿using System;
using System.Collections.Generic;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using CareerCRM.App;
using CareerCRM.App.Interface;
using CareerCRM.App.Request;
using CareerCRM.App.Response;
using CareerCRM.App.SSO;
using CareerCRM.Mvc.Models;

namespace CareerCRM.Mvc.Controllers
{
    public class UserManagerController : BaseController
    {
        private readonly UserManagerApp _app;
        public UserManagerController(IAuth authUtil, UserManagerApp app) : base(authUtil)
        {
            _app = app;
        }
        //
        // GET: /UserManager/
        [Authenticate]
        public ActionResult Index()
        {
            return View();
        }

        //添加或修改组织
       [HttpPost]
        public string AddOrUpdate(UserView view)
        {
            try
            {
                _app.AddOrUpdate(view);

            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        /// <summary>
        /// 加载组织下面的所有用户
        /// </summary>
        public JsonResult Load([FromQuery]QueryUserListReq request)
        {
            return Json(_app.Load(request));
        }

       [HttpPost]
        public string Delete(string[] ids)
        {
            try
            {
                _app.Delete(ids);
            }
            catch (Exception e)
            {
                Result.Code = 500;
                Result.Message = e.InnerException?.Message ?? e.Message;
            }

            return JsonHelper.Instance.Serialize(Result);
        }

        #region 获取权限数据

        /// <summary>
        /// 获取用户可访问的账号
        /// </summary>
        public string GetAccessedUsers()
        {
            IEnumerable<UserView> users = _app.Load(new QueryUserListReq()).data;
            var result = new Dictionary<string, object>();
            foreach (var user in users)
            {
                var item = new
                {
                    Account = user.Account,
                    RealName = user.Name,
                    id = user.Id,
                    text = user.Name,
                    value = user.Account,
                    parentId = "0",
                    showcheck = true,
                    img = "fa fa-user",
                };
                result.Add(user.Id, item);
            }

            return JsonHelper.Instance.Serialize(result);
        }
        #endregion

    }
}