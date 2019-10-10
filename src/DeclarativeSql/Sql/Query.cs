namespace DeclarativeSql.Sql
{
    /// <summary>
    /// Represents a query statement and parameter pair.
    /// </summary>
    public sealed class Query
    {
        #region Properties
        /// <summary>
        /// Gets SQL statement.
        /// </summary>
        public string Statement { get; }


        /// <summary>
        /// Gets bind parameter.
        /// </summary>
        public BindParameter BindParameter { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="bindParameter"></param>
        public Query(string statement, BindParameter bindParameter)
        {
            this.Statement = statement;
            this.BindParameter = bindParameter;
        }
        #endregion
    }
}
