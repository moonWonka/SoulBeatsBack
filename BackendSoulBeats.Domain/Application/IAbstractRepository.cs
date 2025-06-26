using Microsoft.Data.SqlClient;

namespace BackendSoulBeats.Domain.Application
{
    public interface IAbstractRepository
    {
        /// <summary>
        /// Ejecuta una consulta que retorna un solo objeto o null
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a retornar</typeparam>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros para la consulta</param>
        /// <param name="useTransaction">Si debe usar transacción (por defecto false)</param>
        /// <returns>El objeto encontrado o null si no existe</returns>
        Task<T?> QuerySingleAsync<T>(string query, object? parameters = null, bool useTransaction = false);

        /// <summary>
        /// Ejecuta una consulta que retorna una lista de objetos
        /// </summary>
        /// <typeparam name="T">Tipo de los objetos a retornar</typeparam>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros para la consulta</param>
        /// <param name="useTransaction">Si debe usar transacción (por defecto false)</param>
        /// <returns>Colección de objetos encontrados</returns>
        Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null, bool useTransaction = false);

        /// <summary>
        /// Ejecuta una consulta que no retorna datos (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros para la consulta</param>
        /// <param name="useTransaction">Si debe usar transacción (por defecto true)</param>
        /// <returns>Número de filas afectadas</returns>
        Task<int> ExecuteAsync(string query, object? parameters = null, bool useTransaction = true);

        /// <summary>
        /// Ejecuta una consulta que retorna un valor escalar
        /// </summary>
        /// <typeparam name="T">Tipo del valor a retornar</typeparam>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros para la consulta</param>
        /// <param name="useTransaction">Si debe usar transacción (por defecto false)</param>
        /// <returns>El valor escalar resultado de la consulta</returns>
        Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null, bool useTransaction = false);

        /// <summary>
        /// Ejecuta múltiples operaciones en una sola transacción
        /// </summary>
        /// <param name="operations">Array de tuplas con consultas y parámetros</param>
        /// <returns>Número total de filas afectadas</returns>
        Task<int> ExecuteMultipleAsync(params (string query, object? parameters)[] operations);

        /// <summary>
        /// Verifica si la conexión a la base de datos está disponible
        /// </summary>
        /// <returns>True si la conexión es exitosa, false en caso contrario</returns>
        Task<bool> TestConnectionAsync();

        /// <summary>
        /// Obtiene la cadena de conexión configurada
        /// </summary>
        /// <returns>La cadena de conexión</returns>
        string GetConnectionString();

        /// <summary>
        /// Inicializa una nueva conexión y transacción para uso manual
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tupla con la conexión y transacción inicializadas</returns>
        Task<(SqlConnection connection, SqlTransaction transaction)> InitTransactionAsync(CancellationToken cancellationToken = default);
    }
}
