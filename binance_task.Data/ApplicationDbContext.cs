using binance_task.Models;
using Microsoft.EntityFrameworkCore;

namespace binance_task.Data
{
    public class ApiDbContext : DbContext
    {

        public ApiDbContext(DbContextOptions<ApiDbContext> options) 
            :base(options)
        {
            
        }

        public DbSet<Keys> Keys { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
