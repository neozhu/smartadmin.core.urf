using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using URF.Core.EF.Trackable;

// Example: extending IRepository<TEntity> and/or ITrackableRepository<TEntity>, scope: application-wide across all IRepositoryX<TEntity>
namespace Northwind.Repository
{
  public class RepositoryX<TEntity> : TrackableRepository<TEntity>, IRepositoryX<TEntity> where TEntity : class, ITrackable
  {
    public RepositoryX(DbContext context) : base(context)
    {
      
    }

    // Example: adding synchronous Find, scope: application-wide
    public TEntity Find(object[] keyValues, CancellationToken cancellationToken = default)
    {
      return this.Context.Find<TEntity>(keyValues) as TEntity;
    }
  }
}
