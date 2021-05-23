using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartAdmin.Dto;

namespace SmartAdmin.Service.Common
{
  public  interface IExcelService
  {
    Task<DataTable> ReadDataTable(Stream inputSteam, string type = ".xlsx");
    Task<Stream> Export<T>( IEnumerable<T> data, ExpColumnOpts[] colopts = null,string name="Sheet1");
  }
}
