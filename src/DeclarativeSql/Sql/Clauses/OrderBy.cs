using System;
using System.Linq.Expressions;
using System.Text;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;
using DeclarativeSql.Sql.Statements;



namespace DeclarativeSql.Sql.Clauses
{
    /// <summary>
    /// Represents order by clause.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class OrderBy<T> : Clause<T>, IOrderBy<T>
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
        /// <param name="parentStatement"></param>
        /// <param name="property"></param>
        /// <param name="isAscending"></param>
        public OrderBy(IStatement<T> parentStatement, Expression<Func<T, object>> property, bool isAscending)
            : base(parentStatement, null)
        {
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
            this.IsAscending = isAscending;
        }

        
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="parentClause"></param>
        /// <param name="property"></param>
        /// <param name="isAscending"></param>
        public OrderBy(IClause<T> parentClause, Expression<Func<T, object>> property, bool isAscending)
            : base(null, parentClause)
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
            if (this.ParentStatement != null)
            {
                this.ParentStatement.Build(builder, bindParameter);
                builder.AppendLine();
            }
            if (this.ParentClause != null)
            {
                this.ParentClause.Build(builder, bindParameter);
                builder.AppendLine();
            }

            //--- Build body
            var table = TableInfo.Get<T>(this.DbProvider.Database);
            var propertyName = ExpressionHelper.GetMemberName(this.Property);
            var columnName = table.ColumnsByMemberName[propertyName].ColumnName;
            var bracket = this.DbProvider.KeywordBracket;

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
