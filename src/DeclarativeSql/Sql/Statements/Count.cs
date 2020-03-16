using Cysharp.Text;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents count statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Count<T> : Statement<T>, ICount<T>
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        public Count(DbProvider provider)
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
            builder.Append("select count(*) as Count from ");
            builder.Append(this.Table.FullName);
        }
        #endregion
    }
}
