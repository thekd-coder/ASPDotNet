using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.Entities;
using WebApplication.Data;
using Microsoft.AspNetCore.JsonPatch;

namespace WebAPI.Tests
{
    [TestFixture]
    public class WeatherForecastControllerDBTests
    {
        private WeatherForecastControllerDB _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            _context.WeatherForecastReports.AddRange(new List<WeatherForecast>
            {
                new WeatherForecast { Id = 1, Date = new DateOnly(2025, 5, 12), TemperatureC = 25, Summary = "Sunny", City = "Seattle" },
                new WeatherForecast { Id = 2, Date = new DateOnly(2025, 5, 13), TemperatureC = 20, Summary = "Cloudy", City = "Portland" }
            });
            _context.SaveChanges();

            _controller = new WeatherForecastControllerDB(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetWeatherForecast_ReturnsAllWeatherForecasts()
        {
            var result = await _controller.GetWeatherForecast();
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<WeatherForecast>>>());
            var value = result.Value;
            Assert.That(value?.Count(), Is.EqualTo(2));
        }

        [TestCase(1, true)]
        [TestCase(99, false)]
        public async Task GetWeatherForecast_ById_ReturnsExpectedResult(int id, bool exists)
        {
            var result = await _controller.GetWeatherForecast(id);

            if (exists)
            {
                Assert.That(result, Is.InstanceOf<ActionResult<WeatherForecast>>());
                Assert.That(result.Value?.Id, Is.EqualTo(id));
            }
            else
            {
                Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            }
        }

        [TestCase(1, true)]
        [TestCase(99, false)]
        public async Task DeleteWeatherForecast_ById_ReturnsExpectedResult(int id, bool exists)
        {
            var result = await _controller.DeleteWeatherForecast(id);

            if (exists)
            {
                Assert.That(result, Is.InstanceOf<NoContentResult>());
                Assert.That(_context.WeatherForecastReports.Count(), Is.EqualTo(1));
            }
            else
            {
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
            }
        }

        [TestCase(3, "2025-05-14", 22, "Rainy", "San Francisco")]
        public async Task PostWeatherForecast_AddsNewWeatherForecast(int id, string date, int temperatureC, string summary, string city)
        {
            var newForecast = new WeatherForecast
            {
                Id = id,
                Date = DateOnly.Parse(date),
                TemperatureC = temperatureC,
                Summary = summary,
                City = city
            };

            var result = await _controller.PostWeatherForecast(newForecast);

            Assert.That(result, Is.InstanceOf<ActionResult<WeatherForecast>>());
            Assert.That(_context.WeatherForecastReports.Count(), Is.EqualTo(3));
        }

        [TestCase(1, "2025-05-15", 30, "Hot", "Seattle", true)]
        [TestCase(99, "2025-05-15", 30, "Hot", "Unknown", false)]
        public async Task PutWeatherForecast_ById_ReturnsExpectedResult(int id, string date, int temperatureC, string summary, string city, bool exists)
        {
            var updatedForecast = new WeatherForecast
            {
                Id = id,
                Date = DateOnly.Parse(date),
                TemperatureC = temperatureC,
                Summary = summary,
                City = city
            };

            var result = await _controller.PutWeatherForecast(id, updatedForecast);

            if (exists)
            {
                Assert.That(result, Is.InstanceOf<NoContentResult>());
                var forecast = _context.WeatherForecastReports.Find(id);
                Assert.That(forecast.TemperatureC, Is.EqualTo(temperatureC));
            }
            else
            {
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
            }
        }

        [TestCase(1, 35, true)]
        [TestCase(99, 35, false)]
        public async Task PatchWeatherForecast_ById_ReturnsExpectedResult(int id, int newTemperatureC, bool exists)
        {
            var patchDocument = new JsonPatchDocument<WeatherForecast>();
            patchDocument.Replace(w => w.TemperatureC, newTemperatureC);

            var result = await _controller.PatchWeatherForcast(id, patchDocument);

            if (exists)
            {
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var forecast = _context.WeatherForecastReports.Find(id);
                Assert.That(forecast.TemperatureC, Is.EqualTo(newTemperatureC));
            }
            else
            {
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
            }
        }
    }
}
