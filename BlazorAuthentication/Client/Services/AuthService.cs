using BlazorAuthentication.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorAuthentication.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient httpClient;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ILocalStorageService localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorage = localStorage;
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var response = await httpClient.PostAsJsonAsync("api/accounts", registerModel);
            var registerResult = await response.Content.ReadFromJsonAsync<RegisterResult>();

            return registerResult;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var response = await httpClient.PostAsJsonAsync("api/login", loginModel);
            var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();

            if (response.IsSuccessStatusCode == false)
            {
                return loginResult;
            }

            await localStorage.SetItemAsync("authToken", loginResult.Token);
            ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            return loginResult;
        }

        public async Task Logout()
        {
            await localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
