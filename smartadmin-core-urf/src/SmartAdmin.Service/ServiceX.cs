using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using TrackableEntities.Common.Core;
using URF.Core.Services;

namespace SmartAdmin.Service
{
    public class ServiceX<TEntity> : Service<TEntity>, IServiceX<TEntity> where TEntity : class, ITrackable
    {
        private readonly IRepositoryX<TEntity> repository;
    public readonly IDataTableImportMappingService _mappingservice;
    public readonly ILogger<TEntity> _logger;
    protected ServiceX(
      IDataTableImportMappingService mappingservice,
      ILogger<TEntity> logger,
      IRepositoryX<TEntity> repository) : base(repository)
        {
            this.repository = repository;
          _logger = logger;
      _mappingservice = mappingservice;
        }

        public TEntity Find(object[] keyValues, CancellationToken cancellationToken = default)
        {
            return this.repository.Find(keyValues, cancellationToken);
        }
    }
}
