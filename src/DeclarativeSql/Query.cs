using System.Dynamic;



namespace DeclarativeSql
{
    /// <summary>
    /// Represents a query statement / parameter pair.
    /// </summary>
    public sealed class Query
    {
        #region Properties
        /// <summary>
        /// Gets SQL statement.
        /// </summary>
        public string Statement { get; }


        /// <summary>
        /// Gets where clause bind parameters.
        /// </summary>
        public ExpandoObject WhereParameters { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="statement">SQL statement</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal Query(string statement, ExpandoObject whereParameters)
        {
            this.Statement = statement;
            this.WhereParameters = whereParameters;
        }
        #endregion
    }
}