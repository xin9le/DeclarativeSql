namespace DeclarativeSql
{
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

        /*
        /// <summary>
        /// SQL Server Compact
        /// </summary>
        SqlServerCe,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSql,
        */
    }
}