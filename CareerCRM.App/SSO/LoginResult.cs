using Infrastructure;

namespace CareerCRM.App.SSO
{
    public class LoginResult :Response<string>
    {
        public string ReturnUrl;
        public string Token;
    }
}