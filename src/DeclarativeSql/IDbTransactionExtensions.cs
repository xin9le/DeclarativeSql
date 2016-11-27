using System;
using System.Data;
using DeclarativeSql.Transactions;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides extension methods for IDbTransaction interface.
    /// </summary>
    public static class IDbTransactionExtensions
    {
        /// <summary>
        /// Converts the specified database transaction into a scope manageable database transaction.
        /// </summary>
        /// <param name="transaction">Target transaction</param>
        /// <returns>Generated transaction instance</returns>
        public static ScopeTransaction Wrap(this IDbTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return new ScopeTransaction(transaction);
        }
    }
}