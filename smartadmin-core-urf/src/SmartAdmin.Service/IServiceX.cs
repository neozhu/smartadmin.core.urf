using System;
using System.Collections.Generic;
using System.Text;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Services;

namespace SmartAdmin.Service
{
    public interface IServiceX<TEntity> : IService<TEntity>, IRepositoryX<TEntity> where TEntity : class, ITrackable
    {

    }
}
