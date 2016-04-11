using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Profiling.Base
{
    public class RelationalCommandFacade : IRelationalCommand
    {
        private readonly IRelationalCommand _relationalCommand;

        public RelationalCommandFacade(IRelationalCommand relationalCommand)
        {
            _relationalCommand = relationalCommand;
        }

        public void ExecuteNonQuery(IRelationalConnection connection, bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                _relationalCommand.ExecuteNonQuery(connection, manageConnection);
            }
        }

        public async Task ExecuteNonQueryAsync(IRelationalConnection connection,
            CancellationToken cancellationToken = new CancellationToken(),
            bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                await _relationalCommand.ExecuteNonQueryAsync(connection, cancellationToken, manageConnection);
            }
        }

        public object ExecuteScalar(IRelationalConnection connection, bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                return _relationalCommand.ExecuteScalar(connection, manageConnection);
            }
        }

        public async Task<object> ExecuteScalarAsync(IRelationalConnection connection,
            CancellationToken cancellationToken = new CancellationToken(),
            bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                return await _relationalCommand.ExecuteScalarAsync(connection, cancellationToken, manageConnection);
            }
        }

        public RelationalDataReader ExecuteReader(IRelationalConnection connection, bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                return _relationalCommand.ExecuteReader(connection, manageConnection);
            }
        }

        public async Task<RelationalDataReader> ExecuteReaderAsync(IRelationalConnection connection,
            CancellationToken cancellationToken = new CancellationToken(),
            bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                return await _relationalCommand.ExecuteReaderAsync(connection, cancellationToken, manageConnection);
            }
        }

        public string CommandText => _relationalCommand.CommandText;
        public IReadOnlyList<RelationalParameter> Parameters => _relationalCommand.Parameters;
    }
}