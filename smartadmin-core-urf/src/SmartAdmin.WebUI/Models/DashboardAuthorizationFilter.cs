using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SmartAdmin.WebUI.Models
{
  // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
  public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
  {
     public Task<bool> AuthorizeAsync(DashboardContext context)
    {
      return Task.FromResult(true);
    }
  }
}

