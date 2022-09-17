using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorAuthentication.Client.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly AuthenticationStateProvider authProvider;
        private readonly IAuthService authService;

        public RefreshTokenService(AuthenticationStateProvider authProvider, IAuthService authService)
        {
            this.authProvider = authProvider;
            this.authService = authService;
        }

        public async Task<string?> TryRefreshToken()
        {
            var authState = await authProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var exp = user.FindFirst(claim => claim.Type.Equals("exp")).Value;
                var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));

                var timeUTC = DateTime.UtcNow;
                var diff = expTime - timeUTC;

                if (diff.TotalMinutes <= 2)
                {
                    return await authService.RefreshToken();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }            
        }
    }
}
