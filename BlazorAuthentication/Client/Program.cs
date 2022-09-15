using BlazorAuthentication.Client;
using BlazorAuthentication.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services
    .AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IHttpService, HttpService>()
    .AddScoped<IWeatherForecastService, WeatherForecastService>();

await builder.Build().RunAsync();
