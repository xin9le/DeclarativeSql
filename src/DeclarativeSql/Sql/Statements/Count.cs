using Cysharp.Text;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents count statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal readonly struct Count<T> : ISql
    {
        #region ISql implementations
        /// <inheritdoc/>
        public void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            var table = TableInfo.Get<T>(dbProvider.Database);
            builder.Append("select count(*) as Count from ");
            builder.Append(table.FullName);
        }
        #endregion
    }
}
