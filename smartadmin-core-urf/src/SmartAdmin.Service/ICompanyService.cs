using System;
using System.Linq.Expressions;
using SmartAdmin.Entity.Models;
using URF.Core.Abstractions.Services;

namespace SmartAdmin.Service
{
  // Example: extending IService<TEntity> and/or ITrackableRepository<TEntity>, scope: ICustomerService
  public interface ICompanyService : IService<Company>
  {
    // Example: adding synchronous Single method, scope: ICustomerService
    Company Single(Expression<Func<Company, bool>> predicate);
  }
}
