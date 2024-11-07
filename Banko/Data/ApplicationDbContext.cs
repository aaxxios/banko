using Logger.Models;
using Microsoft.EntityFrameworkCore;
namespace Logger.Data
{
    public class ApplicationDbContext: DbContext 
    {
        private IConfiguration _configuration;
        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<Detail> Details { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Default"));
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Production")!);
        }
    }
}
