using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<WeatherForecast> WeatherForecastReports { get; set; }
    }
}

