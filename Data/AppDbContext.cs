using Microsoft.EntityFrameworkCore;
using ai_backend_dotnet.Models;

namespace ai_backend_dotnet.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }
    }
}
