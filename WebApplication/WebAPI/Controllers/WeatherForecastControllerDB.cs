using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebApplication.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastControllerDB : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WeatherForecastControllerDB(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/WeatherForecastControllerDB
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetWeatherForecast()
        {
            return await _context.WeatherForecastReports.ToListAsync();
        }

        // GET: api/WeatherForecastControllerDB/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WeatherForecast>> GetWeatherForecast(int id)
        {
            var weatherForecast = await _context.WeatherForecastReports.FindAsync(id);

            if (weatherForecast == null)
            {
                return NotFound();
            }

            return weatherForecast;
        }

        // PUT: api/WeatherForecastControllerDB/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWeatherForecast(int id, WeatherForecast weatherForecast)
        {
            if (id != weatherForecast.Id)
            {
                return BadRequest();
            }

            _context.Entry(weatherForecast).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeatherForecastExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WeatherForecastControllerDB
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WeatherForecast>> PostWeatherForecast(WeatherForecast weatherForecast)
        {
            _context.WeatherForecastReports.Add(weatherForecast);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWeatherForecast", new { id = weatherForecast.Id }, weatherForecast);
        }

        // DELETE: api/WeatherForecastControllerDB/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeatherForecast(int id)
        {
            var weatherForecast = await _context.WeatherForecastReports.FindAsync(id);
            if (weatherForecast == null)
            {
                return NotFound();
            }

            _context.WeatherForecastReports.Remove(weatherForecast);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/WeatherForecastControllerDB/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchWeatherForcast(int id, [FromBody] JsonPatchDocument<WeatherForecast> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var weatherForecast = await _context.WeatherForecastReports.FindAsync(id);
            if (weatherForecast == null)
            {
                return NotFound();
            }

            // Apply the patch to the entity
            patchDocument.ApplyTo(weatherForecast, (error) => ModelState.AddModelError(string.Empty, error.ErrorMessage));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                /// <summary>
                ///     Saves all changes made in this context to the database.
                /// </summary>
                /// <remarks>
                ///     <para>
                ///         This method will automatically call <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" />
                ///         to discover any changes to entity instances before saving to the underlying database. This can be disabled via
                ///         <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
                ///     </para>
                /// </remarks>
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeatherForecastExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(weatherForecast);
        }

        private bool WeatherForecastExists(int id)
        {
            return _context.WeatherForecastReports.Any(e => e.Id == id);
        }
    }
}
