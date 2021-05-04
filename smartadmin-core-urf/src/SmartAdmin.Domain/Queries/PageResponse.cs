using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAdmin.Application.Queries
{
  public class PageResponse<T>
  {
    public int total { get; set; }
    public IEnumerable<T> rows {get;set;}

  }
}
