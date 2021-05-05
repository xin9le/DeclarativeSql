using System.Runtime.CompilerServices;
using DeclarativeSql.DbOperations;
using Microsoft.Data.SqlClient;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides initializer to use Microsoft.Data.SqlClient specific feature.
    /// </summary>
    internal static class MicrosoftSqlClientInitializer
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        [ModuleInitializer]
        public static void Initialize()
            => DbOperation.Factory[typeof(SqlConnection)] = MicrosoftSqlClientOperation.Create;
    }
}
