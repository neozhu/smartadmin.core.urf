using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartAdmin.Data.Models;

namespace SmartAdmin.Application.Queries
{
   public  class CustomerPaginationQuery:IRequest<PageResponse<Customer>>
    {
    public int Page { get; set; } = 1;
    public int Rows { get; set; } = 10;
    public string Sort { get; set; } = "Id";
    public string Order { get; set; } = "dsc";
    public string FilterRules { get; set; } = "";
    public CustomerPaginationQuery()
    {

    }
    public CustomerPaginationQuery(int page,
                         int rows,
                         string sort,
                         string order,
                         string filterRules)
    {
      this.Page = page;
      this.Rows = rows;
      this.Sort = sort;
      this.Order = order;
      this.FilterRules = filterRules;
    }
    }

  public class GetCustomerById : IRequest<Customer>
  {
    public int Id { get; set; }
    public GetCustomerById()
    {

    }
  }
}
