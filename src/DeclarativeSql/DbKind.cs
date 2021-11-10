namespace DeclarativeSql;



/// <summary>
/// Represents kind of database.
/// </summary>
public enum DbKind
{
    /// <summary>
    /// SQL Server
    /// </summary>
    SqlServer = 0,

    /// <summary>
    /// MySQL / Amazon Aurora / MariaDB
    /// </summary>
    MySql,

    /// <summary>
    /// SQLite
    /// </summary>
    Sqlite,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSql,

    /// <summary>
    /// Oracle
    /// </summary>
    Oracle,
}
