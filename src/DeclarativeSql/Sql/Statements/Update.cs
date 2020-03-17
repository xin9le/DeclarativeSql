using System;
using System.Linq;
using System.Linq.Expressions;
using Cysharp.Text;
using DeclarativeSql.Internals;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents update statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Update<T> : Statement<T>, IUpdate<T>
    {
        #region Properties
        /// <summary>
        /// Gets update target properties.
        /// </summary>
        private Expression<Func<T, object>> Properties { get; }


        /// <summary>
        /// Gets the value priority of ModifiedAt column.
        /// </summary>
        private ValuePriority ModifiedAtPriority { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="properties">Update target properties</param>
        /// <param name="modifiedAtPriority"></param>
        public Update(DbProvider provider, Expression<Func<T, object>> properties, ValuePriority modifiedAtPriority)
            : base(provider)
        {
            this.Properties = properties;
            this.ModifiedAtPriority = modifiedAtPriority;
        }
        #endregion


        #region override
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        internal override void Build(ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            //--- Extract update target columns
            var columns
                = this.Table.Columns
                .Where(x => !x.IsAutoIncrement)
                .Where(x => !x.IsCreatedAt)
                .Where(x => !x.IsModifiedAt);
            if (this.Properties != null)  // Pick up only specified columns
            {
                var propertyNames = ExpressionHelper.GetMemberNames(this.Properties);
                columns = columns.Join(propertyNames, x => x.MemberName, y => y, (x, y) => x);
            }
            foreach (var x in this.Table.Columns.Where(x => x.IsModifiedAt))
                columns = columns.Append(x);

            //--- Build SQL
            var bracket = this.DbProvider.KeywordBracket;
            var prefix = this.DbProvider.BindParameterPrefix;
            builder.Append("update ");
            builder.AppendLine(this.Table.FullName);
            builder.Append("set");
            foreach (var x in columns)
            {
                builder.AppendLine();
                builder.Append("    ");
                builder.Append(bracket.Begin);
                builder.Append(x.ColumnName);
                builder.Append(bracket.End);
                builder.Append(" = ");
                if (x.IsModifiedAt
                    && this.ModifiedAtPriority == ValuePriority.Default
                    && x.DefaultValue != null)
                {
                    builder.Append(x.DefaultValue);
                    builder.Append(',');
                }
                else
                {
                    builder.Append(prefix);
                    builder.Append(x.MemberName);
                    builder.Append(',');
                    //bindParameter.Add(x.MemberName, null);
                }
            }
            builder.Advance(-1);  // remove last colon
        }
        #endregion
    }
}
