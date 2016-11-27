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

        /*
        /// <summary>
        /// SQL Server Compact
        /// </summary>
        SqlServerCe,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,
        */

        /// <summary>
        /// MySQL / Amazon Aurora / MariaDB
        /// </summary>
        MySql,

        /*
        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSql,

        /// <summary>
        /// SQLite
        /// </summary>
        SQLite,
        */
    }
}