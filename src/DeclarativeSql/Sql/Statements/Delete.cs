using Cysharp.Text;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents delete statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Delete<T> : Statement<T>, IDelete<T>
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        public Delete(DbProvider provider)
            : base(provider)
        {}
        #endregion


        #region override
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        internal override void Build(ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            builder.Append("delete from ");
            builder.Append(this.Table.FullName);
        }
        #endregion
    }
}
