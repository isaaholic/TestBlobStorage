using Microsoft.EntityFrameworkCore;
using TestBlobStorage.Models;

namespace TestBlobStorage.Data
{
    public class ServerDbContext:DbContext
    {
        public ServerDbContext(DbContextOptions options):base(options)
        {
            
        }

        // Cosmos Db. If you want to use AzureSql then comment this
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToContainer("Users");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
    }
}
