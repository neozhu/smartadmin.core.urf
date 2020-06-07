using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TrackableEntities.Common.Core;
using URF.Core.Services;

namespace SmartAdmin.Service
{
    public class ServiceX<TEntity> : Service<TEntity>, IServiceX<TEntity> where TEntity : class, ITrackable
    {
        private readonly IRepositoryX<TEntity> repository;

        protected ServiceX(IRepositoryX<TEntity> repository) : base(repository)
        {
            this.repository = repository;
        }

        public TEntity Find(object[] keyValues, CancellationToken cancellationToken = default)
        {
            return this.repository.Find(keyValues, cancellationToken);
        }
    }
}
