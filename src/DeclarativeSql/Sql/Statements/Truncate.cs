using Cysharp.Text;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents truncate statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Truncate<T> : Statement<T>, ITruncate<T>
    {
        #region Constructors
        /// <inheritdoc/>
        public Truncate()
        { }
        #endregion


        #region override
        /// <inheritdoc/>
        internal override void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            var table = TableInfo.Get<T>(dbProvider.Database);
            builder.Append("truncate table ");
            builder.Append(table.FullName);
        }
        #endregion
    }
}
