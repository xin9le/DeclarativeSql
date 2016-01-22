using System;
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
        [DbConnection("System.Data.SqlClient.SqlConnection")]
        [BindParameterPrefix('@')]
        SqlServer,

        /// <summary>
        /// SQL Server Compact
        /// </summary>
        [ProviderName("System.Data.SqlServerCe.4.0")]
        [DbConnection("System.Data.SqlServerCe.SqlCeConnection")]
        [BindParameterPrefix('@')]
        SqlServerCe,

        /// <summary>
        /// Oracle (Managed Driver)
        /// </summary>
        [ProviderName("Oracle.ManagedDataAccess.Client")]
        [DbConnection("Oracle.ManagedDataAccess.Client.OracleConnection")]
        [BindParameterPrefix(':')]
        Oracle,

        /// <summary>
        /// Oracle (Unmanaged Driver)
        /// </summary>
        [ProviderName("Oracle.DataAccess.Client")]
        [DbConnection("Oracle.DataAccess.Client.OracleConnection")]
        [BindParameterPrefix(':')]
        [Obsolete("Please use 'DbKind.Oracle' which supports ODP.NET managed driver.")]
        UnmanagedOracle,

        /// <summary>
        /// MySQL / Amazon Aurora / MariaDB
        /// </summary>
        [ProviderName("MySql.Data.MySqlClient")]
        [DbConnection("MySql.Data.MySqlClient.MySqlConnection")]
        [BindParameterPrefix('@')]
        MySql,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        [ProviderName("Npgsql")]
        [DbConnection("Npgsql.NpgsqlConnection")]
        [BindParameterPrefix(':')]
        PostgreSql,

        /// <summary>
        /// SQLite
        /// </summary>
        [ProviderName("System.Data.SQLite")]
        [DbConnection("System.Data.SQLite.SQLiteConnection")]
        [BindParameterPrefix('@')]
        SQLite,

        /// <summary>
        /// OLE Database
        /// </summary>
        [ProviderName("System.Data.OleDb")]
        [DbConnection("System.Data.OleDb.OleDbConnection")]
        OleDb,

        /// <summary>
        /// ODBC
        /// </summary>
        [ProviderName("System.Data.Odbc")]
        [DbConnection("System.Data.Odbc.OdbcConnection")]
        Odbc,
    }
}