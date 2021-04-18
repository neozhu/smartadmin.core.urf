using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartAdmin.Data.Models;
using URF.Core.Abstractions.Services;

namespace SmartAdmin.Service
{
  public interface IMenuItemService : IService<MenuItem>
  {

    Task<IEnumerable<MenuItem>> GetByParentId(int parentid);
    
    Task ImportDataTableAsync(DataTable datatable);
    Task<Stream> ExportExcelAsync(Expression<Func<MenuItem, bool>> filters, string sort = "Id", string order = "asc");
    Task<IEnumerable<MenuItem>> CreateWithControllerAsync();
    Task<IEnumerable<MenuItem>> ReBuildMenusAsync();
  }
}
