using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Domain.Entities;

namespace WeatherApi.Application.Services
{
    public interface IWeatherService
    {
        Task<List<Weather>> GetAllAsync();
        Task<Weather> GetByIdAsync(int id);
        Task<Weather> CreateAsync(Weather weather);
        Task<int> UpdateAsync(int id, Weather weather);
        Task<int> DeleteAsync(int id);
        Task<Weather> FetchWeatherAsync(string cityName);
    }
}
