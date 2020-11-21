using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cysharp.Text;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents update statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal readonly struct Update<T> : ISql
    {
        #region Properties
        /// <summary>
        /// Gets update target properties.
        /// </summary>
        private Expression<Func<T, object?>>? Properties { get; }


        /// <summary>
        /// Gets the value priority of ModifiedAt column.
        /// </summary>
        private ValuePriority ModifiedAtPriority { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="properties">Update target properties</param>
        /// <param name="modifiedAtPriority"></param>
        public Update(Expression<Func<T, object?>>? properties, ValuePriority modifiedAtPriority)
        {
            this.Properties = properties;
            this.ModifiedAtPriority = modifiedAtPriority;
        }
        #endregion


        #region ISql implementations
        /// <inheritdoc/>
        public void Build(DbProvider dbProvider, TableInfo table, ref Utf16ValueStringBuilder builder, ref BindParameter? bindParameter)
        {
            //--- Extract update target columns
            HashSet<string>? targetMemberNames = null;
            if (this.Properties is not null)
                targetMemberNames = ExpressionHelper.GetMemberNames(this.Properties);

            //--- Build SQL
            var bracket = dbProvider.KeywordBracket;
            var prefix = dbProvider.BindParameterPrefix;
            builder.Append("update ");
            builder.AppendLine(table.FullName);
            builder.Append("set");
            foreach (var x in table.Columns)
            {
                if (x.IsAutoIncrement) continue;
                if (x.IsCreatedAt) continue;

                if (x.IsModifiedAt || targetMemberNames is null || targetMemberNames.Contains(x.MemberName))
                {
                    builder.AppendLine();
                    builder.Append("    ");
                    builder.Append(bracket.Begin);
                    builder.Append(x.ColumnName);
                    builder.Append(bracket.End);
                    builder.Append(" = ");
                    if (x.IsModifiedAt
                        && this.ModifiedAtPriority == ValuePriority.Default
                        && x.DefaultValue is not null)
                    {
                        builder.Append(x.DefaultValue);
                        builder.Append(',');
                    }
                    else
                    {
                        builder.Append(prefix);
                        builder.Append(x.MemberName);
                        builder.Append(',');
                        bindParameter ??= new BindParameter();
                        bindParameter.Add(x.MemberName, null);
                    }
                }
            }
            builder.Advance(-1);  // remove last colon
        }
        #endregion
    }
}
