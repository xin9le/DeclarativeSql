using System;
using System.Data;
using DeclarativeSql.Transactions;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides extension methods for IDbConnection interface.
    /// </summary>
    public static class IDbConnectionExtensions
    {
        /// <summary>
        /// Begins a transaction for the specified database connection.
        /// </summary>
        /// <param name="connection">Database connection for starting transaction</param>
        /// <returns>Generated transaction instance</returns>
        public static ScopeTransaction StartTransaction(this IDbConnection connection)
            => connection.StartTransactionCore(null);


        /// <summary>
        /// Begins a specified isolation level transaction for the specified database connection.
        /// </summary>
        /// <param name="connection">Database connection for starting transaction</param>
        /// <param name="isolationLevel">Isolation level</param>
        /// <returns>Generated transaction instance</returns>
        public static ScopeTransaction StartTransaction(this IDbConnection connection, IsolationLevel isolationLevel)
            => connection.StartTransactionCore(isolationLevel);


        /// <summary>
        /// egins a specified isolation level transaction for the specified database connection.
        /// </summary>
        /// <param name="connection">Database connection for starting transaction</param>
        /// <param name="isolationLevel">Isolation level</param>
        /// <returns>Generated transaction instance</returns>
        private static ScopeTransaction StartTransactionCore(this IDbConnection connection, IsolationLevel? isolationLevel)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var transaction = isolationLevel.HasValue
                            ? connection.BeginTransaction(isolationLevel.Value)
                            : connection.BeginTransaction();
            return transaction.Wrap();
        }
    }
}