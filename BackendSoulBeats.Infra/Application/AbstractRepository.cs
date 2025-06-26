using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Dapper;
using BackendSoulBeats.Domain.Application;

namespace BackendSoulBeats.Infra.Application
{
    public abstract class AbstractRepository : IAbstractRepository
    {
        protected readonly string _connectionString;
        protected readonly ILogger? _logger;

        /// <summary>
        /// Constructor que obtiene connection string desde una variable de entorno específica
        /// </summary>
        /// <param name="environmentVariableName">Nombre de la variable de entorno que contiene la cadena de conexión completa (ej: "CHATDB")</param>
        protected AbstractRepository(string environmentVariableName, ILogger? logger = null)
        {
            _connectionString = Environment.GetEnvironmentVariable(environmentVariableName)
                ?? throw new InvalidOperationException($"Variable de entorno '{environmentVariableName}' no encontrada");
            _logger = logger;
        }

        protected SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<T?> QuerySingleAsync<T>(string query, object? parameters = null, bool useTransaction = false)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en QuerySingleAsync: {ErrorNumber} | Query: {Query}", ex.Number, query);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en QuerySingleAsync | Query: {Query}", query);
                throw;
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null, bool useTransaction = false)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(query, parameters);
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en QueryAsync: {ErrorNumber} | Query: {Query}", ex.Number, query);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en QueryAsync | Query: {Query}", query);
                throw;
            }
        }

        public async Task<int> ExecuteAsync(string query, object? parameters = null, bool useTransaction = true)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en ExecuteAsync: {ErrorNumber} | Query: {Query}", ex.Number, query);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en ExecuteAsync | Query: {Query}", query);
                throw;
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null, bool useTransaction = false)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                var result = await connection.ExecuteScalarAsync<T>(query, parameters);
                return result!;
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en ExecuteScalarAsync: {ErrorNumber} | Query: {Query}", ex.Number, query);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en ExecuteScalarAsync | Query: {Query}", query);
                throw;
            }
        }

        public async Task<int> ExecuteMultipleAsync(params (string query, object? parameters)[] operations)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();
                
                try
                {
                    int totalAffected = 0;
                    foreach (var (query, parameters) in operations)
                    {
                        totalAffected += await connection.ExecuteAsync(query, parameters, transaction);
                    }
                    await transaction.CommitAsync();
                    return totalAffected;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en ExecuteMultipleAsync: {ErrorNumber} | Operations: {OperationCount}", ex.Number, operations.Length);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en ExecuteMultipleAsync | Operations: {OperationCount}", operations.Length);
                throw;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetConnectionString() => _connectionString;

        public async Task<(SqlConnection connection, SqlTransaction transaction)> InitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var transaction = connection.BeginTransaction();
                return (connection, transaction);
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en InitTransactionAsync: {ErrorNumber}", ex.Number);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en InitTransactionAsync");
                throw;
            }
        }
    }
}
