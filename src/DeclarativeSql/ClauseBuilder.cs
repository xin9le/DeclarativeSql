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
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return new CountClause<T>(provider, null);
        }   


        /// <summary>
        /// Builds select clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="properties">Property expressions mapped columns. If null, targets all columns.</param>
        /// <returns>Select clause</returns>
        public static ISelectClause<T> Select<T>(this DbProvider provider, Expression<Func<T, object>> properties = null)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return new SelectClause<T>(provider, null, properties);
        }


        /// <summary>
        /// Builds insert clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="useSequence">Whether use sequence</param>
        /// <param name="setIdentity">Whether set identity</param>
        /// <returns>Insert clause</returns>
        public static IInsertClause<T> Insert<T>(this DbProvider provider, bool useSequence = true, bool setIdentity = false)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return new InsertClause<T>(provider, null, useSequence, setIdentity);
        }


        /// <summary>
        /// Builds update clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="properties">Property expressions mapped columns. If null, targets all columns.</param>
        /// <param name="setIdentity">Whether set identity</param>
        /// <returns>Update clause</returns>
        public static IUpdateClause<T> Update<T>(this DbProvider provider, Expression<Func<T, object>> properties = null, bool setIdentity = false)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return new UpdateClause<T>(provider, null, properties, setIdentity);
        }


        /// <summary>
        /// Builds delete clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <returns>Delete clause</returns>
        public static IDeleteClause<T> Delete<T>(this DbProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return new DeleteClause<T>(provider, null);
        }


        /// <summary>
        /// Builds truncate clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <returns>Truncate clause</returns>
        public static ITruncateClause<T> Truncate<T>(this DbProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return new TruncateClause<T>(provider, null);
        }


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClause<T> Where<T>(this DbProvider provider, Expression<Func<T, bool>> predicate)
        {
            if (provider == null)  throw new ArgumentNullException(nameof(provider));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new WhereClause<T>(provider, null, predicate);
        }

        
        /// <summary>
        /// Builds order by clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IOrderByClause<T> OrderBy<T>(this DbProvider provider, Expression<Func<T, object>> property)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderByClause<T>(provider, null, property, true);
        }

        
        /// <summary>
        /// Builds order by descending clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="provider">Database provider</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IOrderByClause<T> OrderByDescending<T>(this DbProvider provider, Expression<Func<T, object>> property)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderByClause<T>(provider, null, property, false);
        }
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
        {
            if (clause == null)    throw new ArgumentNullException(nameof(clause));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new WhereClause<T>(clause.DbProvider, clause, predicate);
        }


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClauseForSelect<T> Where<T>(this ISelectClause<T> clause, Expression<Func<T, bool>> predicate)
        {
            if (clause == null)    throw new ArgumentNullException(nameof(clause));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new WhereClause<T>(clause.DbProvider, clause, predicate);
        }


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClause<T> Where<T>(this IUpdateClause<T> clause, Expression<Func<T, bool>> predicate)
        {
            if (clause == null)    throw new ArgumentNullException(nameof(clause));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new WhereClause<T>(clause.DbProvider, clause, predicate);
        }


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Where clause</returns>
        public static IWhereClause<T> Where<T>(this IDeleteClause<T> clause, Expression<Func<T, bool>> predicate)
        {
            if (clause == null)    throw new ArgumentNullException(nameof(clause));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new WhereClause<T>(clause.DbProvider, clause, predicate);
        }


        /// <summary>
        /// Builds order by clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IOrderByClause<T> OrderBy<T>(this ISelectClause<T> clause, Expression<Func<T, object>> property)
        {
            if (clause == null)   throw new ArgumentNullException(nameof(clause));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderByClause<T>(clause.DbProvider, clause, property, true);
        }


        /// <summary>
        /// Builds order by descending clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IOrderByClause<T> OrderByDescending<T>(this ISelectClause<T> clause, Expression<Func<T, object>> property)
        {
            if (clause == null)   throw new ArgumentNullException(nameof(clause));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderByClause<T>(clause.DbProvider, clause, property, false);
        }

        
        /// <summary>
        /// Builds order by clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <param name="isAscending">Element order. If true, ascending. If false descending.</param>
        /// <returns>Where clause</returns>
        public static IOrderByClause<T> OrderBy<T>(this IWhereClauseForSelect<T> clause, Expression<Func<T, object>> property, bool isAscending = true)
        {
            if (clause == null)   throw new ArgumentNullException(nameof(clause));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderByClause<T>(clause.DbProvider, clause, property, isAscending);
        }


        /// <summary>
        /// Builds order by descending clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IOrderByClause<T> OrderByDescending<T>(this IWhereClauseForSelect<T> clause, Expression<Func<T, object>> property)
        {
            if (clause == null)   throw new ArgumentNullException(nameof(clause));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderByClause<T>(clause.DbProvider, clause, property, false);
        }

        
        /// <summary>
        /// Builds then by clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IThenByClause<T> ThenBy<T>(this IOrderByClause<T> clause, Expression<Func<T, object>> property)
        {
            if (clause == null)   throw new ArgumentNullException(nameof(clause));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new ThenByClause<T>(clause.DbProvider, clause, property, true);
        }

        
        /// <summary>
        /// Builds then by descending clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IThenByClause<T> ThenByDescending<T>(this IOrderByClause<T> clause, Expression<Func<T, object>> property)
        {
            if (clause == null)   throw new ArgumentNullException(nameof(clause));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new ThenByClause<T>(clause.DbProvider, clause, property, false);
        }

        
        /// <summary>
        /// Builds then by clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IThenByClause<T> ThenBy<T>(this IThenByClause<T> clause, Expression<Func<T, object>> property)
        {
            if (clause == null)   throw new ArgumentNullException(nameof(clause));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new ThenByClause<T>(clause.DbProvider, clause, property, true);
        }

        
        /// <summary>
        /// Builds then by descending clause.
        /// </summary>
        /// <typeparam name="T">Type information of table model.</typeparam>
        /// <param name="clause">Previous clause</param>
        /// <param name="property">Property expression mapped column.</param>
        /// <returns>Where clause</returns>
        public static IThenByClause<T> ThenByDescending<T>(this IThenByClause<T> clause, Expression<Func<T, object>> property)
        {
            if (clause == null)   throw new ArgumentNullException(nameof(clause));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new ThenByClause<T>(clause.DbProvider, clause, property, false);
        }
        #endregion
    }
}