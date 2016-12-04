using System.Dynamic;



namespace DeclarativeSql
{
    /// <summary>
    /// Represents a where clause.
    /// </summary>
    public class WhereClause
    {
        #region Properties
        /// <summary>
        /// Gets SQL statement.
        /// </summary>
        public string Statement { get; }


        /// <summary>
        /// Gets parameters.
        /// </summary>
        public ExpandoObject Parameter { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="statement">SQL statement</param>
        /// <param name="parameter">Query parameters</param>
        internal WhereClause(string statement, ExpandoObject parameter)
        {
            this.Statement = statement;
            this.Parameter = parameter;
        }
        #endregion
    }
}