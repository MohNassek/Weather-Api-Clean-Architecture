using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Domain.Entities;

namespace WeatherApi.Infrastructure.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> dbContextOptions) : base(dbContextOptions)
        {


        }
        public DbSet<Weather> Weathers { get; set; }
    }
}
