using CareerCRM.App;
using CareerCRM.App.Interface;
using CareerCRM.App.Response;
using CareerCRM.Mvc.Models;
using CareerCRM.Repository.Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CareerCRM.Mvc.Controllers
{
    public class ModuleManagerController : BaseController
    {
        private ModuleManagerApp _app;
        public ModuleManagerController(IAuth authUtil, ModuleManagerApp app) : base(authUtil)
        {
            _app = app;
        }

        // GET: /ModuleManager/
        [Authenticate]
        public ActionResult Index()
        {
            return View();
        }

        [Authenticate]
        public ActionResult Assign()
        {
            return View();
        }

        /// <summary>
        /// 加载角色模块
        /// </summary>
        /// <param name="firstId">The role identifier.</param>
        /// <returns>System.String.</returns>
        public string LoadForRole(string firstId)
        {
            System.Collections.Generic.IEnumerable<Module> modules = _app.LoadForRole(firstId);
            return JsonHelper.Instance.Serialize(modules);
        }

        /// <summary>
        /// 根据某角色ID获取可访问某模块的菜单项
        /// </summary>
        /// <returns></returns>
        public string LoadMenusForRole(string moduleId, string firstId)
        {
            System.Collections.Generic.IEnumerable<ModuleElement> menus = _app.LoadMenusForRole(moduleId, firstId);
            return JsonHelper.Instance.Serialize(menus);
        }

        /// <summary>
        /// 获取发起页面的菜单权限
        /// </summary>
        /// <returns>System.String.</returns>
        public string LoadAuthorizedMenus(string modulecode)
        {
            AuthStrategyContext user = _authUtil.GetCurrentUser();
            ModuleView module = user.Modules.FirstOrDefault(u => u.Code == modulecode);
            if (module != null)
            {
                return JsonHelper.Instance.Serialize(module.Elements);

            }
            return "";
        }


        #region 添加编辑模块

        //添加模块
        [HttpPost]

        public string Add(Module model)
        {
            try
            {
                _app.Add(model);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        //修改模块
        [HttpPost]

        public string Update(Module model)
        {
            try
            {
                _app.Update(model);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public string Delete(string[] ids)
        {
            try
            {
                _app.DelModule(ids);
            }
            catch (Exception e)
            {
                Result.Code = 500;
                Result.Message = e.InnerException?.Message ?? e.Message;
            }

            return JsonHelper.Instance.Serialize(Result);
        }

        #endregion 添加编辑模块

        /// <summary>
        /// 加载当前用户可访问模块的菜单
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <returns>System.String.</returns>
        public string LoadMenus(string moduleId)
        {
            AuthStrategyContext user = _authUtil.GetCurrentUser();
            ModuleView module = null;
            if (!string.IsNullOrEmpty(moduleId))
            {
                module = user.Modules.Single(u => u.Id == moduleId);
            }

            TableData data = new TableData
            {
                data = module?.Elements,
                count = module == null ? 0 : module.Elements.Count(),
            };
            return JsonHelper.Instance.Serialize(data);
        }

        //添加功能按钮
        [HttpPost]

        public string AddMenu(ModuleElement model)
        {
            try
            {
                _app.AddMenu(model);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        //添加功能按钮
        [HttpPost]

        public string UpdateMenu(ModuleElement model)
        {
            try
            {
                _app.UpdateMenu(model);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }


        /// <summary>
        /// 删除菜单
        /// </summary>
        [HttpPost]
        public string DelMenu(params string[] ids)
        {
            try
            {
                _app.DelMenu(ids);
            }
            catch (Exception e)
            {
                Result.Code = 500;
                Result.Message = e.InnerException?.Message ?? e.Message;
            }

            return JsonHelper.Instance.Serialize(Result);
        }


    }
}