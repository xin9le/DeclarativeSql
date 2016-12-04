using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using This = DeclarativeSql.DbProvider;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides database functions.
    /// </summary>
    public class DbProvider
    {
        #region Properties
        /// <summary>
        /// Gets kind of database.
        /// </summary>
        public DbKind Kind { get; }


        /// <summary>
        /// Gets assembly name.
        /// </summary>
        public string AssemblyName { get; }


        /// <summary>
        /// Gets database provider factory type name.
        /// </summary>
        internal string FactoryTypeName { get; }


        /// <summary>
        /// Gets DbConnection type name.
        /// </summary>
        public string ConnectionTypeName { get; }


        /// <summary>
        /// Gets bind parameter prefix.
        /// </summary>
        public char? BindParameterPrefix { get; }


        /// <summary>
        /// Gets sql keyword bracket pair.
        /// </summary>
        public BracketPair KeywordBrackets { get; }


        /// <summary>
        /// Gets database provider factory.
        /// </summary>
        public DbProviderFactory Factory
        {
            get
            {
                lock (this)
                {
                    if (this.factory != null)
                        return this.factory;

                    var name = new AssemblyName(this.AssemblyName);
                    var assembly = Assembly.Load(name);
                    var typeInfo = assembly.GetType(this.FactoryTypeName).GetTypeInfo();
                    var field = typeInfo.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
                    this.factory = (DbProviderFactory)field.GetValue(null);
                    return this.factory;
                }
            }
        }
        private DbProviderFactory factory = null;

        
        /// <summary>
        /// Gets sql generator.
        /// </summary>
        public SqlGenerator Sql { get; }        
        #endregion


        #region Constructors / Destructors
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="kind">Kind of database</param>
        /// <param name="assemblyName">Assembly name</param>
        /// <param name="factoryTypeName">Database provider factory type name</param>
        /// <param name="connectionTypeName">DbConnection type name</param>
        /// <param name="bindParameterPrefix">Bind parameter prefix</param>
        /// <param name="keywordBrackets">SQL keyword bracket pair</param>
        private DbProvider(DbKind kind, string assemblyName, string factoryTypeName, string connectionTypeName, char? bindParameterPrefix, BracketPair keywordBrackets)
        {
            this.Kind = kind;
            this.AssemblyName = assemblyName;
            this.FactoryTypeName = factoryTypeName;
            this.ConnectionTypeName = connectionTypeName;
            this.BindParameterPrefix = bindParameterPrefix;
            this.KeywordBrackets = keywordBrackets;
            this.Sql = new SqlGenerator(this);
        }


        /// <summary>
        /// Call when first access.
        /// </summary>
        static DbProvider()
        {
            This.All = new []
            {
                This.SqlServer,
                This.MySql,
                This.Sqlite,
                This.PostgreSql,
                //This.SqlServerCe,
                //This.Oracle,
                //This.UnmanagedOracle,
            };
            This.ByKind = This.All.ToDictionary(x => x.Kind);
            This.ByConnectionTypeName = This.All.ToDictionary(x => x.ConnectionTypeName);
        }
        #endregion


        #region Instances
        /// <summary>
        /// Gets all database providers.
        /// </summary>
        public static IReadOnlyCollection<This> All { get; }


        /// <summary>
        /// Gets all database providers by DbKind.
        /// </summary>
        public static IReadOnlyDictionary<DbKind, This> ByKind { get; }


        /// <summary>
        /// Gets all database providers by ConnectionTypeName.
        /// </summary>
        internal static IReadOnlyDictionary<string, This> ByConnectionTypeName { get; }


        /// <summary>
        /// Gets database provider for SQL Server.
        /// </summary>
        public static This SqlServer { get; } = new This
        (
            DbKind.SqlServer,
            "System.Data.SqlClient",
            "System.Data.SqlClient.SqlClientFactory",
            "System.Data.SqlClient.SqlConnection",
            '@',
            new BracketPair('[', ']')
        );


        /// <summary>
        /// Gets database provider for MySQL / Amazon Aurora / MariaDB.
        /// </summary>
        public static This MySql { get; } = new This
        (
            DbKind.MySql,
            "MySql.Data",
            "MySql.Data.MySqlClient.MySqlClientFactory",
            "MySql.Data.MySqlClient.MySqlConnection",
            '@',
            new BracketPair('`', '`')
        );


        /// <summary>
        /// Gets database provider for SQLite.
        /// </summary>
        public static This Sqlite { get; } = new This
        (
            DbKind.Sqlite,
            "Microsoft.Data.Sqlite",
            "Microsoft.Data.Sqlite.SqliteFactory",
            "Microsoft.Data.Sqlite.SqliteConnection",
            '@',
            new BracketPair('"', '"')
        );


        /// <summary>
        /// Gets database provider for PostgreSQL.
        /// </summary>
        public static This PostgreSql { get; } = new This
        (
            DbKind.PostgreSql,
            "Npgsql",
            "Npgsql.NpgsqlFactory",
            "Npgsql.NpgsqlConnection",
            ':',
            new BracketPair('"', '"')
        );


        /*
        /// <summary>
        /// Gets database provider for SQL Server Compact.
        /// </summary>
        public static This SqlServerCe { get; } = new This
        (
            DbKind.SqlServerCe,
            "",
            "System.Data.SqlServerCe.4.0",
            "System.Data.SqlServerCe.SqlCeConnection",
            '@',
            new BracketPair('[', ']')
        );


        /// <summary>
        /// Gets database provider for Oracle.
        /// </summary>
        public static This Oracle { get; } = new This
        (
            DbKind.Oracle,
            "",
            "Oracle.ManagedDataAccess.Client",
            "Oracle.ManagedDataAccess.Client.OracleConnection",
            ':',
            new BracketPair('"', '"')
        );
        */
        #endregion


        #region Connection
        /// <summary>
        /// Creates database connection.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>IDbConnection instance</returns>
        public IDbConnection CreateConnection(string connectionString = null)
        {
            var connection = this.Factory.CreateConnection();
            if (string.IsNullOrWhiteSpace(connectionString))
                connection.ConnectionString = connectionString;
            return connection;
        }
        #endregion
    }
}