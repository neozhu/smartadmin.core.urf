using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using SmartAdmin.Data.Models;
using URF.Core.Abstractions.Services;

namespace SmartAdmin.Service
{
  public interface IDataTableImportMappingService : IService<DataTableImportMapping>
  {

    Task<IEnumerable<EntityInfo>> GetAssemblyEntitiesAsync();
    Task GenerateByEnityNameAsync(string entityName);

    Task<DataTableImportMapping> FindMappingAsync(string entitySetName, string sourceFieldName);
    Task CreateExcelTemplateAsync(string entityname, string filename);
    Task ImportDataTableAsync(DataTable datatable);
    Task Delete(int[] id);
    Task<Stream> ExportExcelAsync(string filterRules = "", string sort = "Id", string order = "asc");
  }
}
