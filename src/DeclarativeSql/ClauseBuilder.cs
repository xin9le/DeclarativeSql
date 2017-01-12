using System;
using System.Linq.Expressions;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides clause building functions.
    /// </summary>
    public static class ClauseBuilder
    {
        #region Build
        /// <summary>
        /// Builds count clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <returns>Count clause</returns>
        public static ICountClause<T> Count<T>(this DbProvider provider)
            => new CountClause<T>(provider, null);


        /// <summary>
        /// Builds select clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="properties">Property expressions mapped columns. If null, targets all columns.</param>
        /// <returns>Select clause</returns>
        public static ISelectClause<T> Select<T>(this DbProvider provider, Expression<Func<T, object>> properties = null)
            => new SelectClause<T>(provider, null, properties);


        /// <summary>
        /// Builds insert clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="useSequence">Whether use sequence</param>
        /// <param name="setIdentity">Whether set identity</param>
        /// <returns>Insert clause</returns>
        public static IInsertClause<T> Insert<T>(this DbProvider provider, bool useSequence = true, bool setIdentity = false)
            => new InsertClause<T>(provider, null, useSequence, setIdentity);


        /// <summary>
        /// Builds update clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="properties">Property expressions mapped columns. If null, targets all columns.</param>
        /// <param name="setIdentity">Whether set identity</param>
        /// <returns>Update clause</returns>
        public static IUpdateClause<T> Update<T>(this DbProvider provider, Expression<Func<T, object>> properties = null, bool setIdentity = false)
            => new UpdateClause<T>(provider, null, properties, setIdentity);


        /// <summary>
        /// Builds delete clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <returns>Delete clause</returns>
        public static IDeleteClause<T> Delete<T>(this DbProvider provider)
            => new DeleteClause<T>(provider, null);


        /// <summary>
        /// Builds truncate clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <returns>Truncate clause</returns>
        public static ITruncateClause<T> Truncate<T>(this DbProvider provider)
            => new TruncateClause<T>(provider, null);


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClause<T> Where<T>(this DbProvider provider, Expression<Func<T, bool>> predicate)
            => new WhereClause<T>(provider, null, predicate);
        #endregion


        #region Chain
        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClause<T> Where<T>(this ICountClause<T> clause, Expression<Func<T, bool>> predicate)
            => new WhereClause<T>(clause.DbProvider, clause, predicate);


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClause<T> Where<T>(this ISelectClause<T> clause, Expression<Func<T, bool>> predicate)
            => new WhereClause<T>(clause.DbProvider, clause, predicate);


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClause<T> Where<T>(this IUpdateClause<T> clause, Expression<Func<T, bool>> predicate)
            => new WhereClause<T>(clause.DbProvider, clause, predicate);


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClause<T> Where<T>(this IDeleteClause<T> clause, Expression<Func<T, bool>> predicate)
            => new WhereClause<T>(clause.DbProvider, clause, predicate);
        #endregion
    }
}