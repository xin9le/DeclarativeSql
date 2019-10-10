using System.Collections.Generic;
using System.Linq;
using DeclarativeSql.Sql;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides database functions.
    /// </summary>
    public sealed class DbProvider
    {
        #region Properties
        /// <summary>
        /// Gets the database kind.
        /// </summary>
        public DbKind Database { get; }


        /// <summary>
        /// Gets the database connection type.
        /// </summary>
        internal string ConnectionTypeName { get; }


        /// <summary>
        /// Gets the default schema.
        /// </summary>
        public string DefaultSchema { get; }


        /// <summary>
        /// Gets the bind parameter prefix.
        /// </summary>
        public char BindParameterPrefix { get; }


        /// <summary>
        /// Gets the SQL keyword bracket.
        /// </summary>
        public BracketPair KeywordBracket { get; }


        /// <summary>
        /// Gets the query builder.
        /// </summary>
        public QueryBuilder QueryBuilder { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="connectionTypeName"></param>
        /// <param name="defaultSchema"></param>
        /// <param name="bindParameterPrefix"></param>
        /// <param name="keywordBracket"></param>
        private DbProvider(DbKind database, string connectionTypeName, string defaultSchema, char bindParameterPrefix, in BracketPair keywordBracket)
        {
            this.Database = database;
            this.ConnectionTypeName = connectionTypeName;
            this.DefaultSchema = defaultSchema;
            this.BindParameterPrefix = bindParameterPrefix;
            this.KeywordBracket = keywordBracket;
            this.QueryBuilder = new QueryBuilder(this);
        }


        /// <summary>
        /// Call when first access.
        /// </summary>
        static DbProvider()
        {
            All = new []
            {
                SqlServer,
                MySql,
                Sqlite,
                PostgreSql,
                Oracle,
            };
            ByDatabase = All.ToDictionary(x => x.Database);
            ByConnectionTypeName = All.ToDictionary(x => x.ConnectionTypeName);
        }
        #endregion


        #region Instances
        /// <summary>
        /// Gets all database providers.
        /// </summary>
        public static IReadOnlyCollection<DbProvider> All { get; }


        /// <summary>
        /// Gets all database providers by DbKind.
        /// </summary>
        public static IReadOnlyDictionary<DbKind, DbProvider> ByDatabase { get; }


        /// <summary>
        /// Gets all database providers by ConnectionTypeName.
        /// </summary>
        internal static IReadOnlyDictionary<string, DbProvider> ByConnectionTypeName { get; }


        /// <summary>
        /// Gets the SQL Server provider.
        /// </summary>
        public static DbProvider SqlServer { get; } = new DbProvider
        (
            DbKind.SqlServer,
            "System.Data.SqlClient.SqlConnection",
            "dbo",
            '@',
            new BracketPair('[', ']')
        );


        /// <summary>
        /// Gets the MySQL / Amazon Aurora / MariaDB provider.
        /// </summary>
        public static DbProvider MySql { get; } = new DbProvider
        (
            DbKind.MySql,
            "MySql.Data.MySqlClient.MySqlConnection",
            null,
            '@',
            new BracketPair('`', '`')
        );


        /// <summary>
        /// Gets the SQLite provider.
        /// </summary>
        public static DbProvider Sqlite { get; } = new DbProvider
        (
            DbKind.Sqlite,
            "Microsoft.Data.Sqlite.SqliteConnection",
            null,
            '@',
            new BracketPair('"', '"')
        );


        /// <summary>
        /// Gets the PostgreSQL provider.
        /// </summary>
        public static DbProvider PostgreSql { get; } = new DbProvider
        (
            DbKind.PostgreSql,
            "Npgsql.NpgsqlConnection",
            null,
            ':',
            new BracketPair('"', '"')
        );


        /// <summary>
        /// Gets the Oracle provider.
        /// </summary>
        public static DbProvider Oracle { get; } = new DbProvider
        (
            DbKind.Oracle,
            "Oracle.ManagedDataAccess.Client.OracleConnection",
            null,
            ':',
            new BracketPair('"', '"')
        );
        #endregion
    }
}
