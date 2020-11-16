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
        /// Gets the default schema.
        /// </summary>
        public string? DefaultSchema { get; }


        /// <summary>
        /// Gets the bind parameter prefix.
        /// </summary>
        public char BindParameterPrefix { get; }


        /// <summary>
        /// Gets the SQL keyword bracket.
        /// </summary>
        public BracketPair KeywordBracket { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="defaultSchema"></param>
        /// <param name="bindParameterPrefix"></param>
        /// <param name="keywordBracket"></param>
        private DbProvider(DbKind database, string? defaultSchema, char bindParameterPrefix, in BracketPair keywordBracket)
        {
            this.Database = database;
            this.DefaultSchema = defaultSchema;
            this.BindParameterPrefix = bindParameterPrefix;
            this.KeywordBracket = keywordBracket;
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
        /// Gets the SQL Server provider.
        /// </summary>
        public static DbProvider SqlServer { get; } = new DbProvider
        (
            DbKind.SqlServer,
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
            null,
            ':',
            new BracketPair('"', '"')
        );
        #endregion
    }
}
