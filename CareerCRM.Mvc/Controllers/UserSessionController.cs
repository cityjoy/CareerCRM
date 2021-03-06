﻿ 
using System;
using System.Diagnostics;
using System.Linq;
using Infrastructure;
using CareerCRM.App;
using CareerCRM.App.Interface;
using CareerCRM.App.Response;

namespace CareerCRM.Mvc.Controllers
{
    public class UserSessionController : BaseController
    {
        private readonly AuthStrategyContext _authStrategyContext;
        public UserSessionController(IAuth authUtil) : base(authUtil)
        {
            _authStrategyContext = _authUtil.GetCurrentUser();
        }

        public string GetUserName()
        {
            return _authUtil.GetUserName();
        }
        /// <summary>
        /// 获取登录用户可访问的所有模块，及模块的操作菜单
        /// </summary>
        public string GetModulesTree()
        {
            var modules = _authStrategyContext.Modules;
            var moduleTree = modules.GenerateTree(u => u.Id, u => u.ParentId);
            return JsonHelper.Instance.Serialize(moduleTree);
        }

        /// <summary>
        /// datatable结构的模块列表
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        public string GetModulesTable(string pId)
        {
            string cascadeId = ".0.";
            if (!string.IsNullOrEmpty(pId))
            {
                var obj = _authStrategyContext.Modules.SingleOrDefault(u => u.Id == pId);
                if (obj == null)
                    throw new Exception("未能找到指定对象信息");
                cascadeId = obj.CascadeId;
            }

            var query = _authStrategyContext.Modules.Where(u => u.CascadeId.Contains(cascadeId));

            return JsonHelper.Instance.Serialize(new TableData
            {
                data = query.ToList(),
                count = query.Count(),
            });

        }

        /// <summary>
        /// 获取用户可访问的模块列表
        /// </summary>
        public string GetModules()
        {
            return JsonHelper.Instance.Serialize(_authStrategyContext.Modules);
        }

        /// <summary>
        /// 获取登录用户可访问的所有部门
        /// <para>用于树状结构</para>
        /// </summary>
        public string GetOrgs()
        {
            return JsonHelper.Instance.Serialize(_authStrategyContext.Orgs);
        }

        /// <summary>
        /// 加载机构的全部下级机构
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <returns></returns>
        public string GetSubOrgs(string orgId)
        {
            string cascadeId = ".0.";
            if (!string.IsNullOrEmpty(orgId))
            {
                var org = _authStrategyContext.Orgs.SingleOrDefault(u => u.Id == orgId);
                if (org == null)
                {
                    return JsonHelper.Instance.Serialize(new TableData
                    {
                        msg ="未找到指定的节点",
                        code = 500,
                    });
                }
                cascadeId = org.CascadeId;
            }

            var query = _authStrategyContext.Orgs.Where(u => u.CascadeId.Contains(cascadeId));

            return JsonHelper.Instance.Serialize(new TableData
            {
                data = query.ToList(),
                count = query.Count(),
            });
        }

    }
}