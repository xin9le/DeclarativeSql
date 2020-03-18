using Cysharp.Text;



namespace DeclarativeSql.Sql
{
    /// <summary>
    /// Represents SQL for specified mapping table.
    /// </summary>
    public interface ISql
    {
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        Query Build(DbProvider dbProvider);
    }



    /// <summary>
    /// Represents SQL for specified mapping table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class Sql : ISql
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        protected Sql()
        { }
        #endregion


        #region ISql implementations
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public Query Build(DbProvider dbProvider)
        {
            var builder = ZString.CreateStringBuilder();
            try
            {
                BindParameter bindParameter = null;
                this.Build(dbProvider, ref builder, ref bindParameter);
                return new Query(builder.ToString(), bindParameter);
            }
            finally
            {
                builder.Dispose();
            }
        }
        #endregion


        #region abstract
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        internal abstract void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter);
        #endregion
    }
}
