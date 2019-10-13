using DeclarativeSql.Sql.Statements;



namespace DeclarativeSql.Sql.Clauses
{
    /// <summary>
    /// Represents SQL clause.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class Clause<T> : Sql<T>, IClause<T>
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
        /// <param name="parentStatement"></param>
        /// <param name="parentClause"></param>
        protected Clause(IStatement<T> parentStatement, IClause<T> parentClause)
            : base(parentStatement?.DbProvider ?? parentClause?.DbProvider)
        {
            this.ParentStatement
                = parentStatement is Null<T>
                ? null
                : parentStatement as Statement<T>;
            this.ParentClause = parentClause as Clause<T>;
        }
        #endregion
    }
}
