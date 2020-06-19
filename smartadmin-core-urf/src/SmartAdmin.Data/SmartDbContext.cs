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


    #region Business Domain Entity
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual  DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    #endregion
     
  }
}
