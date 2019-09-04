 
using System.Collections.Generic;
using Infrastructure;
using CareerCRM.App.Response;
using CareerCRM.Repository.Domain;

namespace CareerCRM.App
{
    public interface IAuthStrategy 
    {
         List<ModuleView> Modules { get; }

        List<ModuleElement> ModuleElements { get; }

        List<Role> Roles { get; }

         List<Resource> Resources { get; }

         List<Org> Orgs { get; }

         User User
        {
            get;set;
        }

        /// <summary>
        /// 根据模块id获取可访问的模块字段
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        List<KeyDescription> GetProperties(string moduleCode);

    }
}