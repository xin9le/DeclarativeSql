using DeclarativeSql.Annotations;



namespace DeclarativeSql
{
    /// <summary>
    /// データベースの種類を表します。
    /// </summary>
    public enum DbKind
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// SQL Server
        /// </summary>
        [ProviderName("System.Data.SqlClient")]
        [DbConnection("SqlConnection")]
        [BindParameterPrefix('@')]
        SqlServer,

        /// <summary>
        /// SQL Server Compact
        /// </summary>
        [ProviderName("System.Data.SqlServerCe.4.0")]
        [DbConnection("SqlCeConnection")]
        [BindParameterPrefix('@')]
        SqlServerCe,

        /// <summary>
        /// Oracle
        /// </summary>
        [ProviderName("Oracle.DataAccess.Client")]
        [DbConnection("OracleConnection")]
        [BindParameterPrefix(':')]
        Oracle,

        /// <summary>
        /// MySQL
        /// </summary>
        [ProviderName("MySql.Data.MySqlClient")]
        [DbConnection("MySqlConnection")]
        [BindParameterPrefix('@')]
        MySql,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        [ProviderName("Npgsql")]
        [DbConnection("NpgsqlConnection")]
        [BindParameterPrefix(':')]
        PostgreSql,

        /// <summary>
        /// SQLite
        /// </summary>
        [ProviderName("System.Data.SQLite")]
        [DbConnection("SQLiteConnection")]
        [BindParameterPrefix('@')]
        SQLite,

        /// <summary>
        /// OLE Database
        /// </summary>
        [ProviderName("System.Data.OleDb")]
        [DbConnection("OleDbConnection")]
        OleDb,

        /// <summary>
        /// ODBC
        /// </summary>
        [ProviderName("System.Data.Odbc")]
        [DbConnection("OdbcConnection")]
        Odbc,
    }
}