using BlazorAuthentication.Shared;

namespace BlazorAuthentication.Client.Services
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]> GetForecast();
    }
}