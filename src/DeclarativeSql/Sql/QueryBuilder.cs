using System;
using System.Linq.Expressions;
using Cysharp.Text;
using DeclarativeSql.Mapping;
using DeclarativeSql.Sql.Clauses;
using DeclarativeSql.Sql.Statements;



namespace DeclarativeSql.Sql
{
    /// <summary>
    /// Provides query builder.
    /// </summary>
    public struct QueryBuilder<T> : IDisposable
    {
        #region Fields
        private readonly DbProvider dbProvider;
        private readonly TableInfo table;
        private Utf16ValueStringBuilder stringBuilder;
        private BindParameter bindParameter;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        public QueryBuilder(DbProvider provider)
        {
            this.dbProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.table = TableInfo.Get<T>(provider.Database);
            this.stringBuilder = ZString.CreateStringBuilder();
            this.bindParameter = null;
        }
        #endregion


        #region IDisposable implementations
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
            => this.stringBuilder.Dispose();
        #endregion


        /// <summary>
        /// Build query up.
        /// </summary>
        /// <returns></returns>
        public Query Build()
        {
            var statement = this.stringBuilder.ToString();
            return new Query(statement, this.bindParameter);
        }


        #region Statements
        /// <summary>
        /// Builds count statement.
        /// </summary>
        /// <returns></returns>
        public void Count()
        {
            this.AppendLineIfNotEmpty();
            var x = new Count<T>();
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds select statement.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public void Select(Expression<Func<T, object>> properties = null)
        {
            this.AppendLineIfNotEmpty();
            var x = new Select<T>(properties);
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds insert statement.
        /// </summary>
        /// <param name="createdAtPriority"></param>
        /// <returns></returns>
        public void Insert(ValuePriority createdAtPriority = default)
        {
            this.AppendLineIfNotEmpty();
            var x = new Insert<T>(createdAtPriority);
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds upadte statement.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="modifiedAtPriority"></param>
        /// <returns></returns>
        public void Update(Expression<Func<T, object>> properties = null, ValuePriority modifiedAtPriority = default)
        {
            this.AppendLineIfNotEmpty();
            var x = new Update<T>(properties, modifiedAtPriority);
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds delete statement.
        /// </summary>
        /// <returns></returns>
        public void Delete()
        {
            this.AppendLineIfNotEmpty();
            var x = new Delete<T>();
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds truncate statement.
        /// </summary>
        /// <returns></returns>
        public void Truncate()
        {
            this.AppendLineIfNotEmpty();
            var x = new Truncate<T>();
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }
        #endregion


        #region Clauses
        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public void Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            this.AppendLineIfNotEmpty();
            var x = new Where<T>(predicate);
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds ascending order by clause.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public void OrderBy(Expression<Func<T, object>> property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            this.AppendLineIfNotEmpty();
            var x = new OrderBy<T>(property, true);
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds descending order by clause.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public void OrderByDescending(Expression<Func<T, object>> property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            this.AppendLineIfNotEmpty();
            var x = new OrderBy<T>(property, false);
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds ascending then by clause.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public void ThenBy(Expression<Func<T, object>> property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            this.AppendLineIfNotEmpty(',');
            var x = new ThenBy<T>(property, true);
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }


        /// <summary>
        /// Builds descending then by clause.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public void ThenByDescending(Expression<Func<T, object>> property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            this.AppendLineIfNotEmpty(',');
            var x = new ThenBy<T>(property, false);
            x.Build(this.dbProvider, this.table, ref this.stringBuilder, ref this.bindParameter);
        }
        #endregion


        #region Helpers
        /// <summary>
        /// Appends the string representation of a specified value to this instance.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        internal void Append<U>(U value)
            => this.stringBuilder.Append(value);


        /// <summary>
        /// Append default line terminator to the end.
        /// </summary>
        /// <param name="value"></param>
        internal void AppendLine()
            => this.stringBuilder.AppendLine();


        /// <summary>
        /// Appends the string representation of a specified value followed by the default line terminator to the end of this instance.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        internal void AppendLine<U>(U value)
            => this.stringBuilder.AppendLine(value);


        /// <summary>
        /// Append default line terminator to the end, if statement isn't empty.
        /// </summary>
        private void AppendLineIfNotEmpty()
        {
            if (this.stringBuilder.Length > 0)
                this.stringBuilder.AppendLine();
        }


        /// <summary>
        /// Appends the string representation of a specified value followed by the default line terminator to the end of this instance, if statement isn't empty.
        /// </summary>
        private void AppendLineIfNotEmpty(char value)
        {
            if (this.stringBuilder.Length > 0)
                this.stringBuilder.AppendLine(value);
        }
        #endregion
    }



    /// <summary>
    /// Provides query builder.
    /// </summary>
    public static class QueryBuilder
    {
        #region Count
        /// <summary>
        /// Builds count statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public static Query Count<T>(DbProvider dbProvider)
        {
            if (dbProvider is null)
                throw new ArgumentNullException(nameof(dbProvider));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Count();
                return builder.Build();
            }
        }


        /// <summary>
        /// Builds count + where statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Query Count<T>(DbProvider dbProvider, Expression<Func<T, bool>> predicate)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Count();
                builder.Where(predicate);
                return builder.Build();
            }
        }
        #endregion


        #region Select
        /// <summary>
        /// Builds select statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Query Select<T>(DbProvider dbProvider, Expression<Func<T, object>> properties = null)
        {
            if (dbProvider is null)
                throw new ArgumentNullException(nameof(dbProvider));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Select(properties);
                return builder.Build();
            }
        }


        /// <summary>
        /// Builds select + where statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="predicate"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Query Select<T>(DbProvider dbProvider, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Select(properties);
                builder.Where(predicate);
                return builder.Build();
            }
        }
        #endregion


        #region Insert
        /// <summary>
        /// Builds insert statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="createdAtPriority"></param>
        /// <returns></returns>
        public static Query Insert<T>(DbProvider dbProvider, ValuePriority createdAtPriority = default)
        {
            if (dbProvider is null)
                throw new ArgumentNullException(nameof(dbProvider));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Insert(createdAtPriority);
                return builder.Build();
            }
        }
        #endregion


        #region Update
        /// <summary>
        /// Builds update statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="properties"></param>
        /// <param name="modifiedAt"></param>
        /// <returns></returns>
        public static Query Update<T>(DbProvider dbProvider, Expression<Func<T, object>> properties = null, ValuePriority modifiedAt = default)
        {
            if (dbProvider is null)
                throw new ArgumentNullException(nameof(dbProvider));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Update(properties, modifiedAt);
                return builder.Build();
            }
        }


        /// <summary>
        /// Builds update + where statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="predicate"></param>
        /// <param name="properties"></param>
        /// <param name="modifiedAt"></param>
        /// <returns></returns>
        public static Query Update<T>(DbProvider dbProvider, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, ValuePriority modifiedAt = default)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Update(properties, modifiedAt);
                builder.Where(predicate);
                return builder.Build();
            }
        }
        #endregion


        #region Delete
        /// <summary>
        /// Builds delete statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public static Query Delete<T>(DbProvider dbProvider)
        {
            if (dbProvider is null)
                throw new ArgumentNullException(nameof(dbProvider));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Delete();
                return builder.Build();
            }
        }


        /// <summary>
        /// Builds delete + where statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Query Delete<T>(DbProvider dbProvider, Expression<Func<T, bool>> predicate)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Delete();
                builder.Where(predicate);
                return builder.Build();
            }
        }
        #endregion


        #region Truncate
        /// <summary>
        /// Builds truncate statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public static Query Truncate<T>(DbProvider dbProvider)
        {
            if (dbProvider is null)
                throw new ArgumentNullException(nameof(dbProvider));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Truncate();
                return builder.Build();
            }
        }
        #endregion


        #region Where
        /// <summary>
        /// Builds where clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Query Where<T>(DbProvider dbProvider, Expression<Func<T, bool>> predicate)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.Where(predicate);
                return builder.Build();
            }
        }
        #endregion


        #region OrderBy
        /// <summary>
        /// Builds ascending order by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Query OrderBy<T>(DbProvider dbProvider, Expression<Func<T, object>> property)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (property is null) throw new ArgumentNullException(nameof(property));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.OrderBy(property);
                return builder.Build();
            }
        }


        /// <summary>
        /// Builds descending order by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Query OrderByDescending<T>(DbProvider dbProvider, Expression<Func<T, object>> property)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (property is null) throw new ArgumentNullException(nameof(property));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.OrderByDescending(property);
                return builder.Build();
            }
        }
        #endregion


        /*
        #region ThenBy
        /// <summary>
        /// Builds ascending then by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Query ThenBy<T>(DbProvider dbProvider, Expression<Func<T, object>> property)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (property is null) throw new ArgumentNullException(nameof(property));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.ThenBy(property);
                return builder.Build();
            }
        }


        /// <summary>
        /// Builds descending then by clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProvider"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Query ThenByDescending<T>(DbProvider dbProvider, Expression<Func<T, object>> property)
        {
            if (dbProvider is null) throw new ArgumentNullException(nameof(dbProvider));
            if (property is null) throw new ArgumentNullException(nameof(property));

            using (var builder = new QueryBuilder<T>(dbProvider))
            {
                builder.TheThenByDescendingnBy(property);
                return builder.Build();
            }
        }
        #endregion
        */
    }
}
