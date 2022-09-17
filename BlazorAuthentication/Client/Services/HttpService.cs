using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlazorAuthentication.Client.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient httpClient;
        private readonly NavigationManager navigationManager;
        private readonly IAuthService authService;
        private readonly IRefreshTokenService refreshTokenService;

        public HttpService(HttpClient httpClient,
                           NavigationManager navigationManager,
                           IAuthService authService,
                           IRefreshTokenService refreshTokenService)
        {
            this.httpClient = httpClient;
            this.navigationManager = navigationManager;
            this.authService = authService;
            this.refreshTokenService = refreshTokenService;
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            using var response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await authService.Logout();
                navigationManager.NavigateTo("login");
                return default;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        private async Task<T> SendRequestWithRefreshToken<T>(HttpRequestMessage request)
        {
            var token = await refreshTokenService.TryRefreshToken();
            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

            return await SendRequest<T>(request);
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequestWithRefreshToken<T>(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await SendRequestWithRefreshToken<T>(request);
        }
    }
}
