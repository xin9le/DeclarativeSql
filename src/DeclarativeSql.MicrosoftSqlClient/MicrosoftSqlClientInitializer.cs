using DeclarativeSql.DbOperations;
using Microsoft.Data.SqlClient;

namespace DeclarativeSql;



/// <summary>
/// Provides initializer to use Microsoft.Data.SqlClient specific feature.
/// </summary>
public static class MicrosoftSqlClientInitializer
{
    /// <summary>
    /// Initialize.
    /// </summary>
    public static void Initialize()
        => DbOperation.Factory[typeof(SqlConnection)] = MicrosoftSqlClientOperation.Create;
}
