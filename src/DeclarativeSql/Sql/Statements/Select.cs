using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cysharp.Text;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents select statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal readonly struct Select<T> : ISql
    {
        #region Properties
        /// <summary>
        /// Gets the properties mapped to the column.
        /// </summary>
        private Expression<Func<T, object>> Properties { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="properties">Properties that mapped to the target column. If null, all columns are targeted.</param>
        public Select(Expression<Func<T, object>> properties)
            => this.Properties = properties;
        #endregion


        #region ISql implementations
        /// <inheritdoc/>
        public void Build(DbProvider dbProvider, TableInfo table, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            //--- Extract target columns
            HashSet<string> targetMemberNames = null;
            if (this.Properties != null)
                targetMemberNames = ExpressionHelper.GetMemberNames(this.Properties);

            //--- Builds SQL
            var bracket = dbProvider.KeywordBracket;
            builder.Append("select");
            foreach (var x in table.Columns)
            {
                if (targetMemberNames?.Contains(x.MemberName) == false)
                    continue;

                builder.AppendLine();
                builder.Append("    ");
                builder.Append(bracket.Begin);
                builder.Append(x.ColumnName);
                builder.Append(bracket.End);
                builder.Append(" as ");
                builder.Append(x.MemberName);
                builder.Append(',');
            }
            builder.Advance(-1);  //--- remove last colon.
            builder.AppendLine();
            builder.Append("from ");
            builder.Append(table.FullName);
        }   
        #endregion
    }
}
