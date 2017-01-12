using System;
using System.Linq.Expressions;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides query building functions.
    /// </summary>
    public static class QueryBuilder
    {
        /// <summary>
        /// Builds count clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <returns>Count clause</returns>
        public static CountClause<T> Count<T>(this DbProvider provider)
            => new CountClause<T>(provider, null);


        /// <summary>
        /// Builds select clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="properties">Property expressions mapped columns. If null, targets all columns.</param>
        /// <returns>Select clause</returns>
        public static SelectClause<T> Select<T>(this DbProvider provider, Expression<Func<T, object>> properties = null)
            => new SelectClause<T>(provider, null, properties);


        /// <summary>
        /// Builds insert clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="useSequence">Whether use sequence</param>
        /// <param name="setIdentity">Whether set identity</param>
        /// <returns>Insert clause</returns>
        public static InsertClause<T> Insert<T>(this DbProvider provider, bool useSequence = true, bool setIdentity = false)
            => new InsertClause<T>(provider, null, useSequence, setIdentity);


        /// <summary>
        /// Builds update clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="properties">Property expressions mapped columns. If null, targets all columns.</param>
        /// <param name="setIdentity">Whether set identity</param>
        /// <returns>Update clause</returns>
        public static UpdateClause<T> Update<T>(this DbProvider provider, Expression<Func<T, object>> properties = null, bool setIdentity = false)
            => new UpdateClause<T>(provider, null, properties, setIdentity);


        /// <summary>
        /// Builds delete clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <returns>Delete clause</returns>
        public static DeleteClause<T> Delete<T>(this DbProvider provider)
            => new DeleteClause<T>(provider, null);


        /// <summary>
        /// Builds truncate clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <returns>Truncate clause</returns>
        public static TruncateClause<T> Truncate<T>(this DbProvider provider)
            => new TruncateClause<T>(provider, null);


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static WhereClause<T> Where<T>(this DbProvider provider, Expression<Func<T, bool>> predicate)
            => new WhereClause<T>(provider, null, predicate);
    }
}