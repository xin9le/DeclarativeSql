using System.Linq;
using System.Text;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents insert statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Insert<T> : Statement<T>, IInsert<T>
    {
        #region Properties
        /// <summary>
        /// Gets the value priority of CreatedAt column.
        /// </summary>
        private ValuePriority CreatedAtPriority { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="createdAtPriority"></param>
        public Insert(DbProvider provider, ValuePriority createdAtPriority)
            : base(provider)
            => this.CreatedAtPriority = createdAtPriority;
        #endregion


        #region override
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        internal override void Build(StringBuilder builder, BindParameter bindParameter)
        {
            var bracket = this.DbProvider.KeywordBracket;
            var prefix = this.DbProvider.BindParameterPrefix;
            var columns = this.Table.Columns.Where(x => !x.IsAutoIncrement).ToArray();

            builder.AppendLine($"insert into {this.Table.FullName}");
            builder.AppendLine("(");
            foreach (var x in columns)
                builder.AppendLine($"    {bracket.Begin}{x.ColumnName}{bracket.End},");
            builder.AppendLine(")");
            builder.AppendLine("values");
            builder.AppendLine("(");
            foreach (var x in columns)
            {
                if ((x.IsCreatedAt || x.IsModifiedAt)
                    && this.CreatedAtPriority == ValuePriority.Attribute
                    && x.CreatedAt.DefaultValue != null)
                {
                    builder.AppendLine($"    {x.CreatedAt.DefaultValue},");
                }
                else
                {
                    builder.AppendLine($"    {prefix}{x.MemberName},");
                    bindParameter.Add(x.MemberName, null);
                }
            }
            builder.Append(")");
        }
        #endregion
    }
}
