using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Services;

namespace SmartAdmin.Service
{
    public interface IServiceX<TEntity> : IService<TEntity>, IRepositoryX<TEntity> where TEntity : class, ITrackable
    {
    Task ImportDataTableAsync(DataTable datatable, string username = "");
    Task<Stream> ExportExcelAsync(Expression<Func<TEntity, bool>> filters, string sort = "Id", string order = "asc");
    }
}
