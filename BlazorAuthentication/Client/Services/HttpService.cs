using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlazorAuthentication.Client.Services
{
    public class HttpService : IHttpService
    {
        private HttpClient httpClient;
        private NavigationManager navigationManager;
        private IAuthService authService;

        public HttpService(HttpClient httpClient, NavigationManager navigationManager, IAuthService authService)
        {
            this.httpClient = httpClient;
            this.navigationManager = navigationManager;
            this.authService = authService;
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

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T>(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await SendRequest<T>(request);
        }
    }
}
