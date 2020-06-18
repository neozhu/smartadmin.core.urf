using SmartAdmin.Data.Models;
using Microsoft.Extensions.Logging;

// Sample to extend CustomerService, scoped to only CustomerService vs. application wide
namespace SmartAdmin.Service
{
  public class CustomerService : ServiceX<Customer>, ICustomerService
  {
 
    public CustomerService(
      ILogger<Customer> _logger,
      IDataTableImportMappingService _mappingservice,
      IRepositoryX<Customer> repository) : base(_mappingservice,_logger,repository)
    {
      
    }
 

  
  }
}
