using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartAdmin.Infrastructure.Persistence
{
  public static class SmartDbContextSeed
  {
    

    public static async Task SeedSampleDataAsync(SmartDbContext context)
    {
      //Seed, if necessary
      if (!context.MenuItems.Any())
      {

        var home = new Domain.Models.MenuItem()
        {
          Title = "首页",
          Description = "首页",
          Action = "Index",
          Controller = "Home",
          Url = "/Home/Index",
          LineNum = "001",
          Icon = "fal fa-home",
          IsEnabled = true,
          
        };
        var menu1 = new Domain.Models.MenuItem()
        {
          Title = "业务中心",
          Description = "业务中心",
          Action = "#",
          Controller = "#",
          Url = "#",
          LineNum = "003",
          Icon = "fal fa-folders",
          IsEnabled = true,
          Children = new HashSet<Domain.Models.MenuItem>()
          {
            new Domain.Models.MenuItem(){
              Title = "产品主档",
              Description = "产品主档",
              Action = "Index",
              Controller = "Products",
              Url = "/Products/Index",
              LineNum = "301",
              Icon = "",
              IsEnabled = true,
            },
             new Domain.Models.MenuItem(){
              Title = "订单信息",
              Description = "订单信息",
              Action = "Index",
              Controller = "Products",
              Url = "/Orders/Index",
              LineNum = "302",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "照片库",
              Description = "照片库",
              Action = "index",
              Controller = "photos",
              Url = "/photos/index",
              LineNum = "303",
              Icon = "",
              IsEnabled = true,
            },
          }
        };
        var menu2 = new Domain.Models.MenuItem()
        {
          Title = "组织架构",
          Description = "组织架构",
          Action = "Index",
          Controller = "Home",
          Url = "/Home/Index",
          LineNum = "002",
          Icon = "fal fa-window",
          IsEnabled = true,
          Children = new HashSet<Domain.Models.MenuItem>()
          {
            new Domain.Models.MenuItem(){
              Title = "企业信息",
              Description = "企业信息",
              Action = "Index",
              Controller = "Companies",
              Url = "/Companies/Index",
              LineNum = "201",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "客户信息",
              Description = "客户信息",
              Action = "Index",
              Controller = "Customers",
              Url = "/Customers/Index",
              LineNum = "202",
              Icon = "",
              IsEnabled = true,
            },
          }
        };
        var menu3 = new Domain.Models.MenuItem()
        {
          Title = "系统管理",
          Description = "系统管理",
          Action = "#",
          Controller = "#",
          Url = "#",
          LineNum = "009",
          Icon = "fal fa-users-cog",
          IsEnabled = true,
          Children = new HashSet<Domain.Models.MenuItem>()
          {
            new Domain.Models.MenuItem(){
              Title = "租户管理",
              Description = "租户管理",
              Action = "Index",
              Controller = "Tenants",
              Url = "/Tenants/Index",
              LineNum = "901",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "账号管理",
              Description = "账号管理",
              Action = "Index",
              Controller = "AccountManage",
              Url = "/AccountManage/Index",
              LineNum = "902",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "角色管理",
              Description = "角色管理",
              Action = "Index",
              Controller = "RoleManage",
              Url = "/RoleManage/Index",
              LineNum = "903",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "角色授权",
              Description = "角色授权",
              Action = "Index",
              Controller = "rolemenus",
              Url = "/rolemenus/index",
              LineNum = "904",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "导航菜单",
              Description = "导航菜单",
              Action = "Index",
              Controller = "MenuItems",
              Url = "/MenuItems/Index",
              LineNum = "905",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "键值对配置",
              Description = "键值对配置",
              Action = "Index",
              Controller = "CodeItems",
              Url = "/CodeItems/Index",
              LineNum = "906",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "导入&导出配置",
              Description = "导入&导出配置",
              Action = "Index",
              Controller = "DataTableImportMappings",
              Url = "/DataTableImportMappings/Index",
              LineNum = "907",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "系统日志",
              Description = "系统日志",
              Action = "Index",
              Controller = "Logs",
              Url = "/Logs/Index",
              LineNum = "908",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "消息推送",
              Description = "消息推送",
              Action = "Index",
              Controller = "notifications",
              Url = "/notifications/index",
              LineNum = "908",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "WebApi",
              Description = "WebApi",
              Action = "Index",
              Controller = "swagger",
              Url = "/swagger/index.html",
              LineNum = "909",
              Icon = "",
              IsEnabled = true,
            },
            new Domain.Models.MenuItem(){
              Title = "CAP EventBus",
              Description = "CAP EventBus",
              Action = "",
              Controller = "cap",
              Url = "/cap",
              LineNum = "910",
              Icon = "",
              IsEnabled = true,
            },
            
          }
        };
        context.MenuItems.Add(home);
        context.MenuItems.Add(menu3);
        context.MenuItems.Add(menu2);
        context.MenuItems.Add(menu1);
        await context.SaveChangesAsync();
        var items=context.MenuItems.Include(x=>x.Parent).ToList();
        var roles = new string[] { "Admin", "Basic" };
        foreach(var role in roles)
        {
          foreach(var item in items)
          {
            var rolemenu = new Domain.Models.RoleMenu()
            {
               RoleName = role,
               MenuId=item.Id,
               IsEnabled = true,
            };
            if (role == "Basic" && (item.Title == "系统管理" || item?.Parent?.Title== "系统管理")){
              continue;
            }
            context.RoleMenus.Add(rolemenu);
          }
        }
        await context.SaveChangesAsync();
      }
      if (!context.CodeItems.Any())
      {
        context.CodeItems.Add(new  Domain.Models.CodeItem() {  CodeType = "Status",  Code = "initialization", Text = "initialization", Description = "Status of workflow" });
        context.CodeItems.Add(new Domain.Models.CodeItem() { CodeType = "Status", Code = "processing", Text = "processing", Description = "Status of workflow" });
        context.CodeItems.Add(new Domain.Models.CodeItem() { CodeType = "Status", Code = "pending", Text = "pending", Description = "Status of workflow" });
        context.CodeItems.Add(new Domain.Models.CodeItem() { CodeType = "Status", Code = "finished", Text = "finished", Description = "Status of workflow" });
       
        await context.SaveChangesAsync();
      }
       
    }
  }
}
