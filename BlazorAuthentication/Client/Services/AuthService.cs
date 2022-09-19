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

        public AuthService(
            HttpClient httpClient, 
            AuthenticationStateProvider authenticationStateProvider, 
            ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorage = localStorage;
        }

        public async Task<RegisterResponse> Register(RegisterRequest registerModel)
        {
            using var response = await httpClient.PostAsJsonAsync("api/accounts", registerModel);
            var registerResult = await response.Content.ReadFromJsonAsync<RegisterResponse>();

            return registerResult;
        }

        public async Task<AuthResponse> Login(LoginRequest loginModel)
        {
            using var response = await httpClient.PostAsJsonAsync("api/login", loginModel);
            var loginResult = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (response.IsSuccessStatusCode == false)
            {
                return loginResult;
            }

            await localStorage.SetItemAsync("authToken", loginResult.Token);
            await localStorage.SetItemAsync("refreshToken", loginResult.RefreshToken);
            ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Token);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            return loginResult;
        }

        public async Task Logout()
        {
            await localStorage.RemoveItemAsync("authToken");
            await localStorage.RemoveItemAsync("refreshToken");
            ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
            httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<string> RefreshToken()
        {
            var token = await localStorage.GetItemAsync<string>("authToken");
            var refreshToken = await localStorage.GetItemAsync<string>("refreshToken");

            var refreshRequest = new RefreshTokenRequest { Token = token, RefreshToken = refreshToken };
            using var response = await httpClient.PostAsJsonAsync("api/token/refresh", refreshRequest);
            var refreshResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (response.IsSuccessStatusCode == false)
            {
                throw new ApplicationException("Something went wrong during the refresh token action");
            }

            await localStorage.SetItemAsync("authToken", refreshResponse.Token);
            await localStorage.SetItemAsync("refreshToken", refreshResponse.RefreshToken);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", refreshResponse.Token);

            return refreshResponse.Token;
        }
    }
}
