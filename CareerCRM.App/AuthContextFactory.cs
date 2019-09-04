 
using CareerCRM.Repository.Domain;
using CareerCRM.Repository.Interface;

namespace CareerCRM.App
{
    /// <summary>
    ///  加载用户所有可访问的资源/机构/模块
    /// </summary>
    public class AuthContextFactory
    {
        private SystemAuthStrategy _systemAuth;
        private NormalAuthStrategy _normalAuthStrategy;
        private readonly IUnitWork _unitWork;

        public AuthContextFactory(SystemAuthStrategy sysStrategy
            , NormalAuthStrategy normalAuthStrategy
            , IUnitWork unitWork)
        {
            _systemAuth = sysStrategy;
            _normalAuthStrategy = normalAuthStrategy;
            _unitWork = unitWork;
        }

        public AuthStrategyContext GetAuthStrategyContext(string username)
        {
            IAuthStrategy service = null;
            service = _normalAuthStrategy;
            service.User = _unitWork.FindSingle<User>(u => u.Account == username);

            // if (username == "System")
            //{
            //    service= _systemAuth;
            //}
            //else
            //{
            //    service = _normalAuthStrategy;
            //    service.User = _unitWork.FindSingle<User>(u => u.Account == username);
            //}

            return new AuthStrategyContext(service);
        }
    }
}