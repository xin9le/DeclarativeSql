using System.Data.SqlClient;
using DeclarativeSql.DbOperations;

namespace DeclarativeSql;



/// <summary>
/// Provides initializer to use System.Data.SqlClient specific feature.
/// </summary>
public static class SystemSqlClientInitializer
{
    /// <summary>
    /// Initialize.
    /// </summary>
    public static void Initialize()
        => DbOperation.Factory[typeof(SqlConnection)] = SystemSqlClientOperation.Create;
}
