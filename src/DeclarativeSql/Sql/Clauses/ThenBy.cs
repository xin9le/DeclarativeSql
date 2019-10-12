using System;
using System.Linq.Expressions;
using System.Text;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Clauses
{
    /// <summary>
    /// Represents then by clause.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ThenBy<T> : Clause<T>, IThenBy<T>
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
        /// <param name="orderBy"></param>
        /// <param name="property"></param>
        /// <param name="isAscending"></param>
        public ThenBy(IOrderBy<T> orderBy, Expression<Func<T, object>> property, bool isAscending)
            : base(null, orderBy)
        {
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
            this.IsAscending = isAscending;
        }


        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="thenBy"></param>
        /// <param name="property"></param>
        /// <param name="isAscending"></param>
        public ThenBy(IThenBy<T> thenBy, Expression<Func<T, object>> property, bool isAscending)
            : base(null, thenBy)
        {
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
            this.IsAscending = isAscending;
        }
        #endregion


        #region override
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        internal override void Build(StringBuilder builder, BindParameter bindParameter)
        {
            //--- Build parent
            if (this.ParentClause != null)
            {
                this.ParentClause.Build(builder, bindParameter);
                builder.AppendLine(",");
            }

            //--- Build body
            var table = TableInfo.Get<T>(this.DbProvider.Database);
            var propertyName = ExpressionHelper.GetMemberName(this.Property);
            var columnName = table.ColumnsByMemberName[propertyName].ColumnName;
            var bracket = this.DbProvider.KeywordBracket;

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
