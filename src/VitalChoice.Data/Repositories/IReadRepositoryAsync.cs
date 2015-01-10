﻿using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Data.Repositories
{
    public interface IReadRepositoryAsync<TEntity> : IReadRepository<TEntity> where TEntity : IObjectState
    {
       /* Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);*/
    }
}