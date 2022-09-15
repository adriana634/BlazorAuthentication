using BlazorAuthentication.Shared;

namespace BlazorAuthentication.Client.Services
{
    public interface IAuthService
    {
        public bool Authenticated { get; }
        Task<RegisterResult> Register(RegisterModel registerModel);
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
    }
}