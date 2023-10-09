using Microsoft.EntityFrameworkCore;
using TestBlobStorage.Models;

namespace TestBlobStorage.Data
{
    public class ServerDbContext:DbContext
    {
        public ServerDbContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
