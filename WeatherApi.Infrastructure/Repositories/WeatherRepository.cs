using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using WeatherApi.Domain.Entities;
using WeatherApi.Infrastructure.Data;
using WeatherApi.Domain.Interface;

namespace WeatherApi.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public WeatherRepository(WeatherDbContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Weather>> GetAllAsync()
        {
            return await _dbContext.Weathers.AsNoTracking().ToListAsync();
        }

        public async Task<Weather> GetByIdAsync(int id)
        {
            return await _dbContext.Weathers.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Weather> CreateAsync(Weather weather)
        {
            if (weather == null) throw new ArgumentNullException(nameof(weather));

            await _dbContext.Weathers.AddAsync(weather);
            await _dbContext.SaveChangesAsync();
            return weather;
        }

        public async Task<int> UpdateAsync(int id, Weather weather)
        {
            if (weather == null) throw new ArgumentNullException(nameof(weather));

            return await _dbContext.Weathers
                .Where(w => w.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(w => w.City, weather.City)
                    .SetProperty(w => w.Country, weather.Country)
                    .SetProperty(w => w.Temperature, weather.Temperature));
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _dbContext.Weathers
                .Where(w => w.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<Weather> FetchWeatherAsync(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                throw new ArgumentException("City name cannot be null or empty.", nameof(cityName));

            string apiKey = "18fc9643295e444acc923d3aa2cb3e23";
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch weather data for city: {cityName}. Error: {response.ReasonPhrase}");
            }

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Deserialize with error handling
            var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (weatherData == null || weatherData.Main == null || weatherData.Sys == null)
            {
                throw new InvalidOperationException("Failed to parse weather data. Response might be incomplete.");
            }

            Weather weather = new Weather
            {
                City = weatherData.Name,
                Country = weatherData.Sys.Country,
                Temperature = weatherData.Main.Temp,
            };

            // Save to database
            await CreateAsync(weather);
            return weather;
        }

        // Inner class to match API response
        private record WeatherApiResponse
        {
            public string Name { get; init; }
            public SysData Sys { get; init; }
            public MainData Main { get; init; }

            public record SysData
            {
                [JsonPropertyName("country")]
                public string Country { get; init; }
            }

            public record MainData
            {
                [JsonPropertyName("temp")]
                public double Temp { get; init; }
            }
        }
    }
}
