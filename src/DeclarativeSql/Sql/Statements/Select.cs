﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DeclarativeSql.Internals;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents select statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Select<T> : Statement<T>, ISelect<T>
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
        /// <param name="provider"></param>
        /// <param name="properties">Properties that mapped to the target column. If null, all columns are targeted.</param>
        public Select(DbProvider provider, Expression<Func<T, object>> properties)
            : base(provider)
            => this.Properties = properties;
        #endregion


        #region override
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        internal override void Build(StringBuilder builder, BindParameter bindParameter)
        {
            //--- Extract target columns
            var columns
                = this.Properties == null
                ? this.Table.Columns
                : this.Table.Columns.Join
                (
                    ExpressionHelper.GetMemberNames(this.Properties),
                    x => x.MemberName,
                    y => y,
                    (x, y) => x
                );
            
            //--- Builds SQL
            var bracket = this.DbProvider.KeywordBracket;
            builder.Append("select");
            foreach (var x in columns)
            {
                builder.AppendLine();
                builder.Append($"    {bracket.Begin}{x.ColumnName}{bracket.End} as {x.MemberName},");
            }
            builder.Length--;  //--- remove last colon.
            builder.AppendLine();
            builder.Append($"from {this.Table.FullName}");
        }   
        #endregion
    }
}