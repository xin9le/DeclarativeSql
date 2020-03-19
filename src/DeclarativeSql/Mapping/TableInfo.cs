using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using DeclarativeSql.Annotations;
using DeclarativeSql.Internals;



namespace DeclarativeSql.Mapping
{
    /// <summary>
    /// Provides table mapping information.
    /// </summary>
    internal sealed class TableInfo
    {
        #region Properties
        /// <summary>
        /// Gets the kind of database.
        /// </summary>
        public DbKind Database { get; private set; }


        /// <summary>
        /// Gets the type that is mapped to the table.
        /// </summary>
        public Type Type { get; private set; }


        /// <summary>
        /// Gets the schema name.
        /// </summary>
        public string Schema { get; private set; }


        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// Gets the full table name.
        /// </summary>
        public string FullName { get; private set; }


        /// <summary>
        /// Gets the column mapping information.
        /// </summary>
        public ReadOnlyArray<ColumnInfo> Columns { get; private set; }


        /// <summary>
        /// Gets the column mapping information by member name.
        /// </summary>
        public FrozenStringKeyDictionary<ColumnInfo> ColumnsByMemberName { get; private set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        private TableInfo()
        {}
        #endregion


        #region Get
        /// <summary>
        /// Gets the table mapping information corresponding to the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TableInfo Get<T>(DbKind database)
            => Cache<T>.Instances.TryGetValue(database, out var value)
            ? value
            : throw new NotSupportedException();
        #endregion


        #region Internal Cache
        /// <summary>
        /// Provides <see cref="TableInfo"/> cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static class Cache<T>
        {
            /// <summary>
            /// Gets the instances by <see cref="DbKind"/>.
            /// </summary>
            public static FrozenDictionary<DbKind, TableInfo> Instances { get; }


            /// <summary>
            /// Static constructors
            /// </summary>
            static Cache()
            {
                var type = typeof(T);
                var flags = BindingFlags.Instance | BindingFlags.Public;
                var attributes = type.GetCustomAttributes<TableAttribute>(true).ToDictionary(x => x.Database);
                var properties = type.GetProperties(flags);
                var fields = type.GetFields(flags);
                Instances
                    = Enum.GetValues(typeof(DbKind))
                    .Cast<DbKind>()
                    .Select(db =>
                    {
                        attributes.TryGetValue(db, out var tableAttr);
                        var provider = DbProvider.ByDatabase[db];
                        var b = provider.KeywordBracket;
                        var schema = tableAttr?.Schema ?? provider.DefaultSchema;
                        var name = tableAttr?.Name ?? type.Name;
                        var columns
                            = properties.Select(x => new ColumnInfo(db, x))
                            .Concat(fields.Select(x => new ColumnInfo(db, x)))
                            .ToReadOnlyArray();
                        var table = new TableInfo
                        {
                            Database = db,
                            Type = type,
                            Schema = schema,
                            Name = name,
                            FullName
                                = string.IsNullOrEmpty(schema)
                                ? ZString.Concat(b.Begin, name, b.End)
                                : ZString.Concat(b.Begin, schema, b.End, '.', b.Begin, name, b.End),
                            Columns = columns,
                            ColumnsByMemberName = columns.ToFrozenStringKeyDictionary(x => x.MemberName),
                        };
                        return (db, table);
                    })
                    .ToFrozenDictionary(x => x.db, x => x.table);
            }
        }
        #endregion
    }
}
