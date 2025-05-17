using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Immutable;
using WebAPI.Entities;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string[] Cities = new[]
        {
            "Amravati", "Pune", "Mumbai", "Nagpur", "New Delhi", "Jaipur", "Manali", "Shimla"
        };

        private List<WeatherForecast> _weatherForecastReports = new();

        private List<WeatherForecast> gatherData()
        {
            int temp = 18;
            return Enumerable.Range(1, 5).Select(index => fillData
            (
                index,
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                temp++,
                Summaries[index],
                Cities[index]
            ))
            .ToList();
        }

        private WeatherForecast fillData(int id, DateOnly dO, int temp, string summary, string city)
        {
            return new WeatherForecast
            {
                Id = id,
                Date = dO,
                TemperatureC = temp,
                Summary = summary,
                City = city
            };
        }

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;

            if (_weatherForecastReports.Count == 0)
            {
                _weatherForecastReports = gatherData();
            }
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<IEnumerable<WeatherForecast>> Get()
        {
            return _weatherForecastReports;
        }

        [HttpGet("{id}")]
        public ActionResult<WeatherForecast> GetByID(int id)
        {
            var WeatherForcastReport = _weatherForecastReports.FirstOrDefault(b => b.Id == id);

            if (WeatherForcastReport == null)
            {
                return NotFound();
            }

            return WeatherForcastReport;
        }

        [HttpPost]
        public ActionResult<WeatherForecast> CreateWeatherReport(WeatherForecast WFReport)
        {
            int index = WFReport.Id = _weatherForecastReports.Count + 1;
            _weatherForecastReports.Add(fillData(
                index,
                WFReport.Date,
                WFReport.TemperatureC,
                WFReport.Summary,
                WFReport.City
                ));

            return CreatedAtAction(nameof(GetByID), new { id = WFReport.Id }, WFReport);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateWeatherForcastReport(int id, WeatherForecast WFReport)
        {
            if (id != WFReport.Id)
            {
                return BadRequest();
            }

            var existingWFReport = _weatherForecastReports.FirstOrDefault(p => p.Id == id);
            if (existingWFReport == null)
            {
                return NotFound();
            }

            _weatherForecastReports.Insert(WFReport.Id - 1, WFReport);  //Update the list

            return CreatedAtAction(nameof(GetByID), new { id = WFReport.Id }, WFReport);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteWeatherForcastReport(int id)
        {
            var existingWFReport = _weatherForecastReports.FirstOrDefault(p => p.Id == id);

            if(existingWFReport == null)
            {
                return NotFound();
            }

            _weatherForecastReports.Remove(existingWFReport);

            return Accepted();
        }
    }
}
