using System;
using System.Linq.Expressions;
using Cysharp.Text;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Clauses
{
    /// <summary>
    /// Represents order by clause.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal readonly struct OrderBy<T> : ISql
    {
        #region Properties
        /// <summary>
        /// Gets the expression for the property mapped to the column.
        /// </summary>
        private Expression<Func<T, object>> Property { get; }


        /// <summary>
        /// Gets whether ascending order.
        /// </summary>
        private bool IsAscending { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="isAscending"></param>
        public OrderBy(Expression<Func<T, object>> property, bool isAscending)
        {
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
            this.IsAscending = isAscending;
        }
        #endregion


        #region ISql implementations
        /// <inheritdoc/>
        public void Build(DbProvider dbProvider, TableInfo table, ref Utf16ValueStringBuilder builder, ref BindParameter? bindParameter)
        {
            var propertyName = ExpressionHelper.GetMemberName(this.Property);
            var columnName = table.ColumnsByMemberName[propertyName].ColumnName;
            var bracket = dbProvider.KeywordBracket;

            builder.AppendLine("order by");
            builder.Append("    ");
            builder.Append(bracket.Begin);
            builder.Append(columnName);
            builder.Append(bracket.End);
            if (!this.IsAscending)
                builder.Append(" desc");
        }
        #endregion
    }
}
