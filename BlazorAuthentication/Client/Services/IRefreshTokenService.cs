namespace BlazorAuthentication.Client.Services
{
    public interface IRefreshTokenService
    {
        Task<string?> TryRefreshToken();
    }
}