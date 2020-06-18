using System;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartAdmin.Data.Models;
using URF.Core.Abstractions.Services;

namespace SmartAdmin.Service
{
  // Example: extending IService<TEntity> and/or ITrackableRepository<TEntity>, scope: ICustomerService
  public interface ICustomerService : IService<Customer>
  {
    
  }
}
