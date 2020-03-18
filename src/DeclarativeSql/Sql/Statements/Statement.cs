namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents SQL statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class Statement<T> : Sql, IStatement<T>
    {
        /// <summary>
        /// Creates instance.
        /// </summary>
        protected Statement()
        { }
    }
}
