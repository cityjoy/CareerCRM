﻿using System;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using CareerCRM.App;
using CareerCRM.App.Interface;
using CareerCRM.App.Request;
using CareerCRM.App.SSO;

namespace CareerCRM.Mvc.Controllers
{
    public class RelevanceManagerController : BaseController
    {
        private readonly RevelanceManagerApp _app;

        public RelevanceManagerController(IAuth authUtil, RevelanceManagerApp app) : base(authUtil)
        {
            _app = app;
        }

        [HttpPost]
        public string Assign(AssignReq request)
        {
            var result = new Response();
            try
            {
                _app.Assign(request);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return JsonHelper.Instance.Serialize(result);
        }
        [HttpPost]
        public string UnAssign(string type, string firstId, string[] secIds)
        {
            try
            {
                _app.UnAssign(type, firstId, secIds);
            }
            catch (Exception ex)
            {
                  Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

       
    }
}