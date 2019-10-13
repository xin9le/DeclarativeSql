using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents SQL statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class Statement<T> : Sql<T>, IStatement<T>
    {
        #region Properties
        /// <summary>
        /// Gets the table mapping information.
        /// </summary>
        protected TableInfo Table { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        protected Statement(DbProvider provider)
            : base(provider)
            => this.Table = TableInfo.Get<T>(provider.Database);
        #endregion
    }
}
