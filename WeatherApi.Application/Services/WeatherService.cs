using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApi.Domain.Entities;
using WeatherApi.Domain.Interface;


namespace WeatherApi.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _weatherRepository;

        public WeatherService(IWeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }

        public async Task<Weather> CreateAsync(Weather weather)
        {
            return await _weatherRepository.CreateAsync(weather);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _weatherRepository.DeleteAsync(id);
        }

        public async Task<Weather> FetchWeatherAsync(string cityName)
        {
            return await _weatherRepository.FetchWeatherAsync(cityName);
        }

        public async Task<List<Weather>> GetAllAsync()
        {
            return await _weatherRepository.GetAllAsync();
        }

        public async Task<Weather> GetByIdAsync(int id)
        {
            return await _weatherRepository.GetByIdAsync(id);
        }

        public async Task<int> UpdateAsync(int id, Weather weather)
        {
            return await _weatherRepository.UpdateAsync(id, weather);
        }
    }
}
