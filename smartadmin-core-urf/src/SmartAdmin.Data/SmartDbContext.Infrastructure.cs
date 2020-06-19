using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SmartAdmin.Data.Models;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Data.Models
{
  public partial class SmartDbContext : DbContext
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SmartDbContext(
      DbContextOptions options,
      IHttpContextAccessor httpContextAccessor) : base(options)
    {
      _httpContextAccessor = httpContextAccessor;
    }
    #region Infrastructure Entity
    public virtual DbSet<DataTableImportMapping> DataTableImportMappings { get; set; }
    public virtual DbSet<CodeItem> CodeItems { get; set; }
    public virtual DbSet<MenuItem> MenuItems { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<RoleMenu> RoleMenus { get; set; }
    #endregion
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
       
      var currentDateTime = DateTime.Now;
      var claimsidentity = (ClaimsIdentity)this._httpContextAccessor.HttpContext.User.Identity;
      var tenantclaim = claimsidentity?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid");
      var tenantid = Convert.ToInt32(tenantclaim?.Value);
      foreach (var auditableEntity in this.ChangeTracker.Entries<Entity>())
      {
        if (auditableEntity.State == EntityState.Added || auditableEntity.State == EntityState.Modified)
        {
          //auditableEntity.Entity.LastModifiedDate = currentDateTime;
          switch (auditableEntity.State)
          {
            case EntityState.Added:
              auditableEntity.Property("LastModifiedDate").IsModified = false;
              auditableEntity.Property("LastModifiedBy").IsModified = false;
              auditableEntity.Entity.CreatedDate = currentDateTime;
              auditableEntity.Entity.CreatedBy = claimsidentity.Name;
              auditableEntity.Entity.TenantId = tenantid;
              break;
            case EntityState.Modified:
              auditableEntity.Property("CreatedDate").IsModified = false;
              auditableEntity.Property("CreatedBy").IsModified = false;
              auditableEntity.Entity.LastModifiedDate = currentDateTime;
              auditableEntity.Entity.LastModifiedBy = claimsidentity.Name;
              auditableEntity.Entity.TenantId = tenantid;

              break;
          }
        }
      }

      return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseSqlServer(@"Server=(LocalDb)\\MSSQLLocalDB;Database=SmartDb;Trusted_Connection=True;");
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      #region Business Domain Entity
      modelBuilder.Entity<Company>(entity =>
      {
        entity.HasIndex(e => e.Name)
                      .IsUnique()
                      .HasFilter(null);
        entity.HasIndex(e => e.Code)
                      .IsUnique()
                      .HasFilter("[Code] IS NOT NULL");
      });
      modelBuilder.Entity<Product>(entity =>
      {
        entity.HasIndex(e => new { e.Name })
                      .IsUnique()
                      .HasFilter(null);

      });
      modelBuilder.Entity<Customer>(entity =>
      {
        entity.HasIndex(e => new { e.Name })
                      .IsUnique()
                      .HasFilter(null);

      });
      modelBuilder.Entity<Order>(entity =>
      {
        entity.HasIndex(e => new { e.OrderNo })
                      .IsUnique()
                      .HasFilter(null);

      });
      #endregion

      #region infrastructure
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
  
      modelBuilder.Entity<RoleMenu>(entity =>
      {
        entity.HasIndex(e => new { e.RoleName, e.MenuId })
                      .IsUnique()
                      .HasFilter(null);

      });
      #endregion
    }
  }
}
