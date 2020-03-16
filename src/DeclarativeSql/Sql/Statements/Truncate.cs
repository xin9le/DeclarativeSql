using Cysharp.Text;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents truncate statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Truncate<T> : Statement<T>, ITruncate<T>
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        public Truncate(DbProvider provider)
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
            builder.Append("truncate table ");
            builder.Append(this.Table.FullName);
        }
        #endregion
    }
}
