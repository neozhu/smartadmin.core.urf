using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SmartAdmin.Data.Models;

namespace SmartAdmin.Data
{
    public class SmartDbContextFactory : IDesignTimeDbContextFactory<SmartDbContext>
    {
        public SmartDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartDbContext>();
            optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=SmartDb;Trusted_Connection=True;");
            return new SmartDbContext(optionsBuilder.Options);
        }
    }
}
