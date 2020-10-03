using System.Threading;
using SmartAdmin.Data.Models;
using URF.Core.Abstractions.Trackable;
using URF.Core.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using Microsoft.Extensions.Logging;
using URF.Core.EF;
using System.Collections.Generic;
using SmartAdmin.Dto;

namespace SmartAdmin.Service
{
  public class RoleMenuService : Service<RoleMenu>, IRoleMenuService
  {

    private readonly ITrackableRepository<MenuItem> _menurepository;
    public RoleMenuService(ITrackableRepository<RoleMenu> repository,
      ITrackableRepository<MenuItem> menurepository)
        : base(repository)
    {
      this._menurepository = menurepository;
    }

    public async Task<IEnumerable<RoleMenu>> GetByMenuIdAsync(int menuid) =>await this.Queryable().Where(x=>x.MenuId== menuid).ToListAsync();

    public async Task<IEnumerable<RoleMenu>> GetByRoleNameAsync(string roleName) => await this.Queryable().Where(x => x.RoleName == roleName).ToListAsync();

    public async Task AuthorizeAsync(RoleMenusView[] list)
    {
      var rolename = list.First().RoleName;
      var menuid = list.First().MenuId;
      var mymenus = await this.GetByRoleNameAsync(rolename);
      foreach (var item in mymenus)
      {
        this.Delete(item);
      }

      if (menuid > 0)
      {
        foreach (var item in list)
        {
          var rm = new RoleMenu
          {
            MenuId = item.MenuId,
            RoleName = item.RoleName,
            Create = item.Create,
            Delete = item.Delete,
            Edit = item.Edit,
            Import = item.Import,
            Export = item.Export,
            FunctionPoint1 = item.FunctionPoint1,
            FunctionPoint2 = item.FunctionPoint2,
            FunctionPoint3 = item.FunctionPoint3
          };
          this.Insert(rm);
        }
      }




    }

    private async Task FindParentMenus(List<MenuItem> list, MenuItem item)
    {
      if (item.ParentId != null && item.ParentId > 0)
      {
        var pitem =await this._menurepository.FindAsync(item.ParentId);
        if (!list.Where(x => x.Id == pitem.Id).Any())
        {
          list.Add(pitem);
        }
        if (pitem.ParentId != null && pitem.ParentId > 0)
        {
           await this.FindParentMenus(list, pitem);
        }
      }
    }

    public async Task<IEnumerable<MenuItem>> RenderMenus(string[] roleNames)
    {
      var allmenus = await this._menurepository.Queryable().OrderBy(n => n.LineNum).ToListAsync();
      var mymenus = await this.Queryable().Where(x => roleNames.Contains(x.RoleName)).ToListAsync();
      var menulist = new List<MenuItem>();
      foreach (var item in allmenus)
      {
        var myitem = mymenus.Where(x => x.MenuId == item.Id).Any();
        if (myitem)
        {
          if (!menulist.Where(x => x.Id == item.Id).Any())
          {
            menulist.Add(item);
          }
          if (item.ParentId != null && item.ParentId > 0)
          {
            await this.FindParentMenus(menulist, item);
          }
        }
      }
      return menulist;
    }

    public async Task<IEnumerable<ListItem>> NavDataSource(string[] roles) {
      var menus =await this._menurepository.Queryable()
          .OrderBy(x => x.LineNum).ToListAsync();
      var owneritems = await this.Queryable()
        .Where(x => roles.Contains(x.RoleName)).ToListAsync();
      var list = new List<ListItem>();
      foreach (var menu in menus.Where(x => x.ParentId == null))
      {
        if (owneritems.Where(x => x.MenuId == menu.Id).Any())
        {
          var nav = new ListItem()
          {

            Href = $"{menu.Controller}_{menu.Action}",
            Icon = menu.Icon,
            Roles = menu.Roles?.Split(','),
            Title = menu.Title,
            Text=menu.Title,
            Route=menu.Controller,
            Tags= menu.Title,
            I18n= $"{menu.Controller}_{menu.Action}",
            Target=menu.Target,
            Disabled=!menu.IsEnabled
          };

          if (menus.Where(x => x.ParentId == menu.Id).Any())
          {
            await fillItems(nav, menu, menus, owneritems);
          }
          list.Add(nav);
        }

      }
    return list;
     }
    private async Task fillItems(ListItem nav, MenuItem parent, IEnumerable<MenuItem> menus, IEnumerable<RoleMenu> owneritems)
    {
      var items = menus.Where(x => x.ParentId == parent.Id).ToList();
      var list = new List<ListItem>();
      foreach (var menu in items)
      {
        if (owneritems.Where(x => x.MenuId == menu.Id).Any())
        {
          var subnav = new ListItem()
          {
            Href = $"{menu.Controller}_{menu.Action}",
            Icon = menu.Icon,
            Roles = menu.Roles?.Split(','),
            Title = menu.Title,
            Text = menu.Title,
            Route = menu.Url,
            Tags = menu.Title,
            Target=menu.Target,
            I18n = $"{menu.Controller}_{menu.Action}"
          };

          if (menus.Where(x => x.ParentId == menu.Id).Any())
          {
            await fillItems(subnav, menu, menus, owneritems);
          }
          list.Add(subnav);
        }
      }
      nav.Items = list;
    }
  }
}



