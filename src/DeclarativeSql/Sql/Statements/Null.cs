using System.Text;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents null statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Null<T> : Statement<T>
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        public Null(DbProvider provider)
            : base(provider)
        {}
        #endregion


        #region override
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        internal override void Build(StringBuilder builder, ref BindParameter bindParameter)
        {}
        #endregion
    }
}
