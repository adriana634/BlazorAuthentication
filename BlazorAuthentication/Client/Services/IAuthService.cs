using BlazorAuthentication.Shared;

namespace BlazorAuthentication.Client.Services
{
    public interface IAuthService
    {
        Task<RegisterResponse> Register(RegisterRequest registerModel);
        Task<AuthResponse> Login(LoginRequest loginModel);
        Task Logout();
        Task<string> RefreshToken();
    }
}