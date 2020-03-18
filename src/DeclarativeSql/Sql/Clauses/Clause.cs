using DeclarativeSql.Sql.Statements;



namespace DeclarativeSql.Sql.Clauses
{
    /// <summary>
    /// Represents SQL clause.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class Clause<T> : Sql, IClause<T>
    {
        #region Properties
        /// <summary>
        /// Gets the parent statement.
        /// </summary>
        protected Statement<T> ParentStatement { get; }


        /// <summary>
        /// Gets the parent clause.
        /// </summary>
        protected Clause<T> ParentClause { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="parent"></param>
        protected Clause(IStatement<T> parent)
        {
            this.ParentStatement
                = parent is Null<T>
                ? null
                : parent as Statement<T>;
            this.ParentClause = null;
        }


        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="parent"></param>
        protected Clause(IClause<T> parent)
        {
            this.ParentStatement = null;
            this.ParentClause = parent as Clause<T>;
        }
        #endregion
    }
}
