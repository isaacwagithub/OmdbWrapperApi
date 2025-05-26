using Microsoft.EntityFrameworkCore;
using OmdbWrapperApi.Models; // Assuming you put CachedMovie in Models folder

namespace OmdbWrapperApi.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options)
        {
        }

        public DbSet<CachedMovie> CachedMovies { get; set; }
    }
}
