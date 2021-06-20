using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartAdmin.Domain.Models;
using SmartAdmin.Service.Common;

namespace SmartAdmin.Service.Implementation
{
  public class PhotoService:ServiceX<Photo>
  {
    private readonly IDataTableImportMappingService mappingservice;
    private readonly ILogger<Photo> logger;
    public PhotoService(
      IDataTableImportMappingService mappingservice,
      ILogger<Photo> logger,
      IExcelService excelService,
      IRepositoryX<Photo> repository) : base(mappingservice, logger, excelService, repository)
    {
      this.mappingservice = mappingservice;
      this.logger = logger;
    }
  }
}
