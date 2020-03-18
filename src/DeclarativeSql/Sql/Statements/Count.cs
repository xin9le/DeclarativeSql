using Cysharp.Text;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents count statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Count<T> : Statement<T>, ICount<T>
    {
        #region Constructors
        /// <inheritdoc/>
        public Count()
        { }
        #endregion


        #region override
        /// <inheritdoc/>
        internal override void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            var table = TableInfo.Get<T>(dbProvider.Database);
            builder.Append("select count(*) as Count from ");
            builder.Append(table.FullName);
        }
        #endregion
    }
}
