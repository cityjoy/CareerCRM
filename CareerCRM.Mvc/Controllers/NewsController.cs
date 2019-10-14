

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using CareerCRM.App.Interface;
using CareerCRM.App.Request;
using CareerCRM.App.Response;
using CareerCRM.Repository.Core;
using CareerCRM.App;
using CareerCRM.Mvc.Models;
using CareerCRM.Repository.Domain;

namespace CareerCRM.Mvc.Controllers
{
    [Authenticate]
    public partial class NewsController : BaseController
    {

        private readonly NewsApp _app;
        public NewsController(IAuth authUtil, NewsApp app) : base(authUtil)
        {
            _app = app;
        }
        #region 首页

        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 加载列表
        public JsonResult Load([FromQuery]NewsListReq request)
        {
            return Json(_app.Load(request));
        }
        #endregion

        #region 新建
        //public ActionResult Create()
        //{
        //      return View();
        //}

        [HttpPost]
        public JsonResult Add(News model)
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
            return Json(Result);
        }
        #endregion

        #region 修改
        public ActionResult Edit(string id)
        {
            var vm = _app.Get(id);
            return View(vm);
        }

        [HttpPost]
        public JsonResult Update(NewsFormDTO model)
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
            return Json(Result);

        }
        #endregion

        #region 删除

        [HttpPost]
        public JsonResult Delete(string[] ids)
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

            return Json(Result);
        }

        #endregion

        #region 详细


        public ActionResult Detail(string id)
        {
            var vm = _app.Get(id);
            return View(vm);

        }

        public JsonResult Get(string id)
        {
            var vm = _app.Get(id);
            return Json(vm);

        }
        #endregion
    }
}


