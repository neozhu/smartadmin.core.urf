using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.Domain.Models;
using URF.Core.EF.Trackable;

namespace SmartAdmin.Infrastructure.Persistence
{
  public partial class SmartDbContext : DbContext
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly int _tenantId;
    private readonly string _userName;
    public SmartDbContext(
      DbContextOptions options,
      IHttpContextAccessor httpContextAccessor) : base(options)
    {
      _httpContextAccessor = httpContextAccessor;
      var claimsidentity = (ClaimsIdentity)this._httpContextAccessor?.HttpContext?.User.Identity;
      var tenantclaim = claimsidentity?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid");
      _tenantId = Convert.ToInt32(tenantclaim?.Value);
      _userName = claimsidentity?.Name;
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
              auditableEntity.Entity.CreatedBy = _userName;
              auditableEntity.Entity.TenantId = _tenantId;
              break;
            case EntityState.Modified:
              auditableEntity.Property("CreatedDate").IsModified = false;
              auditableEntity.Property("CreatedBy").IsModified = false;
              auditableEntity.Entity.LastModifiedDate = currentDateTime;
              auditableEntity.Entity.LastModifiedBy = _userName;
              auditableEntity.Entity.TenantId = _tenantId;

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

    
  }
}
