using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace VitalChoice.Profiling.Base
{
    public class RelationalCommandFacade : IRelationalCommand
    {
        private readonly IRelationalCommand _relationalCommand;

        public RelationalCommandFacade(IRelationalCommand relationalCommand)
        {
            _relationalCommand = relationalCommand;
        }

        public int ExecuteNonQuery(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues = null, bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                return _relationalCommand.ExecuteNonQuery(connection, parameterValues, manageConnection);
            }
        }

        public async Task<int> ExecuteNonQueryAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues = null, bool manageConnection = true,
            CancellationToken cancellationToken = new CancellationToken())
        {
            using (new ProfilingScope(CommandText))
            {
                return await _relationalCommand.ExecuteNonQueryAsync(connection, parameterValues, manageConnection, cancellationToken);
            }
        }

        public object ExecuteScalar(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues = null, bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                return _relationalCommand.ExecuteScalar(connection, parameterValues, manageConnection);
            }
        }

        public async Task<object> ExecuteScalarAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues = null, bool manageConnection = true,
            CancellationToken cancellationToken = new CancellationToken())
        {
            using (new ProfilingScope(CommandText))
            {
                return await _relationalCommand.ExecuteScalarAsync(connection, parameterValues, manageConnection, cancellationToken);
            }
        }

        public RelationalDataReader ExecuteReader(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues = null,
            bool manageConnection = true)
        {
            using (new ProfilingScope(CommandText))
            {
                return _relationalCommand.ExecuteReader(connection, parameterValues, manageConnection);
            }
        }

        public async Task<RelationalDataReader> ExecuteReaderAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues = null, bool manageConnection = true,
            CancellationToken cancellationToken = new CancellationToken())
        {
            using (new ProfilingScope(CommandText))
            {
                return await _relationalCommand.ExecuteReaderAsync(connection, parameterValues, manageConnection, cancellationToken);
            }
        }

        public string CommandText => _relationalCommand.CommandText;

        IReadOnlyList<IRelationalParameter> IRelationalCommand.Parameters => _relationalCommand.Parameters;
    }
}