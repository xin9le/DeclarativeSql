using System;
using System.Linq.Expressions;
using DeclarativeSql.Sql.Clauses;
using DeclarativeSql.Sql.Statements;



namespace DeclarativeSql.Sql
{
    /// <summary>
    /// Provides query builder.
    /// </summary>
    public sealed class QueryBuilder
    {
        #region Properties
        /// <summary>
        /// Gets database provider.
        /// </summary>
        internal DbProvider DbProvider { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        internal QueryBuilder(DbProvider provider)
            => this.DbProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        #endregion


        #region Statement
        /// <summary>
        /// Builds count statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICount<T> Count<T>()
            => new Count<T>(this.DbProvider);


        /// <summary>
        /// Builds select statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="properties"></param>
        /// <returns></returns>
        public ISelect<T> Select<T>(Expression<Func<T, object>> properties = null)
            => new Select<T>(this.DbProvider, properties);


        /// <summary>
        /// Builds insert statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createdAtPriority"></param>
        /// <returns></returns>
        public IInsert<T> Insert<T>(ValuePriority createdAtPriority = ValuePriority.Attribute)
            => new Insert<T>(this.DbProvider, createdAtPriority);


        /// <summary>
        /// Builds upadte statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="properties"></param>
        /// <returns></returns>
        public IUpdate<T> Update<T>(Expression<Func<T, object>> properties = null)
            => new Update<T>(this.DbProvider, properties);


        /// <summary>
        /// Builds delete statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDelete<T> Delete<T>()
            => new Delete<T>(this.DbProvider);


        /// <summary>
        /// Builds truncate statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ITruncate<T> Truncate<T>()
            => new Truncate<T>(this.DbProvider);
        #endregion
    }



    /// <summary>
    /// Provides <see cref="QueryBuilder"/> extension methods.
    /// </summary>
    public static class QueryBuilderExtensions
    {
        #region Where
        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IWhere<T> Where<T>(this QueryBuilder builder, Expression<Func<T, bool>> predicate)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var @null = new Null<T>(builder.DbProvider);
            return new Where<T>(@null, predicate);
        }


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IWhere<T> Where<T>(this ICount<T> count, Expression<Func<T, bool>> predicate)
        {
            if (count == null) throw new ArgumentNullException(nameof(count));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new Where<T>(count, predicate);
        }


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IWhereForSelect<T> Where<T>(this ISelect<T> select, Expression<Func<T, bool>> predicate)
        {
            if (select == null) throw new ArgumentNullException(nameof(select));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new Where<T>(select, predicate);
        }


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="update"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IWhere<T> Where<T>(this IUpdate<T> update, Expression<Func<T, bool>> predicate)
        {
            if (update == null) throw new ArgumentNullException(nameof(update));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new Where<T>(update, predicate);
        }


        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delete"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IWhere<T> Where<T>(this IDelete<T> delete, Expression<Func<T, bool>> predicate)
        {
            if (delete == null) throw new ArgumentNullException(nameof(delete));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new Where<T>(delete, predicate);
        }
        #endregion


        #region OrderBy
        /// <summary>
        /// Builds ascending order by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderBy<T> OrderBy<T>(this QueryBuilder builder, Expression<Func<T, object>> property)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            var @null = new Null<T>(builder.DbProvider);
            return new OrderBy<T>(@null, property, true);
        }


        /// <summary>
        /// Builds descending order by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderBy<T> OrderByDescending<T>(this QueryBuilder builder, Expression<Func<T, object>> property)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            var @null = new Null<T>(builder.DbProvider);
            return new OrderBy<T>(@null, property, false);
        }


        /// <summary>
        /// Builds ascending order by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderBy<T> OrderBy<T>(this ISelect<T> select, Expression<Func<T, object>> property)
        {
            if (select == null) throw new ArgumentNullException(nameof(select));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderBy<T>(select, property, true);
        }


        /// <summary>
        /// Builds descending order by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderBy<T> OrderByDescending<T>(this ISelect<T> select, Expression<Func<T, object>> property)
        {
            if (select == null) throw new ArgumentNullException(nameof(select));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderBy<T>(select, property, false);
        }


        /// <summary>
        /// Builds ascending order by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderBy<T> OrderBy<T>(this IWhereForSelect<T> where, Expression<Func<T, object>> property)
        {
            if (where == null) throw new ArgumentNullException(nameof(where));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderBy<T>(where, property, true);
        }


        /// <summary>
        /// Builds descending order by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderBy<T> OrderByDescending<T>(this IWhereForSelect<T> where, Expression<Func<T, object>> property)
        {
            if (where == null) throw new ArgumentNullException(nameof(where));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new OrderBy<T>(where, property, false);
        }
        #endregion


        #region ThenBy
        /// <summary>
        /// Builds ascending then by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IThenBy<T> ThenBy<T>(this IOrderBy<T> orderBy, Expression<Func<T, object>> property)
        {
            if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new ThenBy<T>(orderBy, property, true);
        }


        /// <summary>
        /// Builds descending then by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IThenBy<T> ThenByDescending<T>(this IOrderBy<T> orderBy, Expression<Func<T, object>> property)
        {
            if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
            if (property == null) throw new ArgumentNullException(nameof(property));
            return new ThenBy<T>(orderBy, property, false);
        }
        #endregion
    }
}
