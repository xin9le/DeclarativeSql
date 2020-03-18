using Cysharp.Text;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents delete statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Delete<T> : Statement<T>, IDelete<T>
    {
        #region Constructors
        /// <inheritdoc/>
        public Delete()
        { }
        #endregion


        #region override
        /// <inheritdoc/>
        internal override void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            var table = TableInfo.Get<T>(dbProvider.Database);
            builder.Append("delete from ");
            builder.Append(table.FullName);
        }
        #endregion
    }
}
