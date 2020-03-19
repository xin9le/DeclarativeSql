using Cysharp.Text;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents truncate statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal readonly struct Truncate<T> : ISql
    {
        #region ISql implementations
        /// <inheritdoc/>
        public void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            var table = TableInfo.Get<T>(dbProvider.Database);
            builder.Append("truncate table ");
            builder.Append(table.FullName);
        }
        #endregion
    }
}
