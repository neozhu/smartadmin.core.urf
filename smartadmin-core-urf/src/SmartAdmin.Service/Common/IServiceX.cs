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
    Task ImportData(Stream stream);
    Task<Stream> Export(Expression<Func<TEntity, bool>> filters, string sort = "Id", string order = "asc");
    Task<TEntity> CreateOrEdit(TEntity entity);
    }
}
