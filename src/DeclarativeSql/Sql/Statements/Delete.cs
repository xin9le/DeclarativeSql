using Cysharp.Text;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents delete statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal readonly struct Delete<T> : ISql
    {
        #region ISql implementations
        /// <inheritdoc/>
        public void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            var table = TableInfo.Get<T>(dbProvider.Database);
            builder.Append("delete from ");
            builder.Append(table.FullName);
        }
        #endregion
    }
}
