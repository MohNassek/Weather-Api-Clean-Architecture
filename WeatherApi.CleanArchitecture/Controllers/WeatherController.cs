using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.Services;
using WeatherApi.Domain.Entities;

namespace WeatherApi.CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // GET: api/weather
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var weatherList = await _weatherService.GetAllAsync();
            return Ok(weatherList);
        }

        // GET: api/weather/get?id={id}
        [HttpGet("get")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var weather = await _weatherService.GetByIdAsync(id);
            if (weather == null)
            {
                return NotFound();
            }
            return Ok(weather);
        }

        // POST: api/weather
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Weather weather)
        {
            var createdWeather = await _weatherService.CreateAsync(weather);
            return CreatedAtAction(nameof(GetById), new { id = createdWeather.Id }, createdWeather);
        }

        // PUT: api/weather/update?id={id}
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] int id, [FromBody] Weather updatedWeather)
        {
            if (updatedWeather == null)
            {
                return BadRequest("Weather data is required.");
            }

            int result = await _weatherService.UpdateAsync(id, updatedWeather);
            if (result == 0)
            {
                return NotFound(); // Return 404 if no records are updated
            }
            return NoContent(); // Return 204 No Content for successful update
        }

        // DELETE: api/weather/delete?id={id}
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            int result = await _weatherService.DeleteAsync(id);
            if (result == 0)
            {
                return NotFound(); // Return 404 if the record is not found
            }
            return NoContent(); // Return 204 No Content for successful deletion
        }

        // GET: api/weather/fetch?cityName={cityName}
        [HttpGet("fetch")]
        public async Task<IActionResult> FetchWeather([FromQuery] string cityName)
        {
            try
            {
                var weather = await _weatherService.FetchWeatherAsync(cityName);
                return Ok(weather);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
