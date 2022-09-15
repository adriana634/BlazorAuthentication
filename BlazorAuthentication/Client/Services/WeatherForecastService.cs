using BlazorAuthentication.Shared;

namespace BlazorAuthentication.Client.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IHttpService httpService;

        public WeatherForecastService(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<WeatherForecast[]> GetForecast()
        {
            return await httpService.Get<WeatherForecast[]>("WeatherForecast");
        }
    }
}
