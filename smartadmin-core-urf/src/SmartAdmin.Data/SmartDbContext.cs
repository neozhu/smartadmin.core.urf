using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SmartAdmin.Data.Models;

namespace SmartAdmin.Data.Models
{
  public partial class SmartDbContext : DbContext
  {
    public SmartDbContext(DbContextOptions options) : base(options)
    {

    }
    #region 基础框架
    public DbSet<DataTableImportMapping> DataTableImportMappings { get; set; }
    public DbSet<CodeItem> CodeItems { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    #endregion
    public DbSet<Company> Companies { get; set; }
    public DbSet<Category>  Categories { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseSqlServer(@"Server=(LocalDb)\\MSSQLLocalDB;Database=SmartDb;Trusted_Connection=True;");
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Company>(entity =>
      {
        entity.HasIndex(e => e.Name)
                      .IsUnique()
                      .HasFilter(null);
        entity.HasIndex(e => e.Code)
                      .IsUnique()
                      .HasFilter("[Code] IS NOT NULL");
      });
      modelBuilder.Entity<Category>(entity =>
      {
        entity.HasIndex(e => e.Name)
                      .IsUnique()
                      .HasFilter(null);
        
      });
      modelBuilder.Entity<DataTableImportMapping>(entity =>
      {
        entity.HasIndex(e => new { e.EntitySetName, e.FieldName })
                      .IsUnique()
                      .HasFilter(null);

      });
      modelBuilder.Entity<CodeItem>(entity =>
      {
        entity.HasIndex(e => new { e.CodeType, e.Code })
                      .IsUnique()
                      .HasFilter(null);

      });
    }
  }
}
