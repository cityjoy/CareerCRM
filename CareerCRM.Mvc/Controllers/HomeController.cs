using Microsoft.AspNetCore.Mvc;
using CareerCRM.App.Interface;
using CareerCRM.App.SSO;

namespace CareerCRM.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IAuth authUtil) : base(authUtil)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }
       
       
        
        public ActionResult Git()
        {
            return View();
        }



        public ActionResult ArticleList()
        {
            return View();
        }

    }
}