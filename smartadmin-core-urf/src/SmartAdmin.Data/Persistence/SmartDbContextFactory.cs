using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SmartAdmin.Infrastructure;

namespace SmartAdmin.Infrastructure.Persistence
{
    public class SmartDbContextFactory : IDesignTimeDbContextFactory<SmartDbContext>
    {
        public SmartDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartDbContext>();
            optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=SmartDb;Trusted_Connection=True;");
            return new SmartDbContext(optionsBuilder.Options,null);
        }
    }
}
