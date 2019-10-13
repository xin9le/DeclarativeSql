using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;
using DeclarativeSql.Sql;



namespace DeclarativeSql.DbOperations
{
    /// <summary>
    /// Provides SQL Server specific database operation.
    /// </summary>
    internal class SqlServerOperation : DbOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="provider"></param>
        /// <param name="timeout"></param>
        public SqlServerOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
            : base(connection, transaction, provider, timeout)
        { }
        #endregion


        #region InsertAndGetId
        /// <summary>
        /// Inserts the specified data into the table and returns the automatically incremented ID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns>Auto incremented ID</returns>
        public override long InsertAndGetId<T>(T data, ValuePriority createdAt)
        {
            var query = this.DbProvider.QueryBuilder.Insert<T>(createdAt).Build();
            var sql = ToInsertAndGetIdSql(query.Statement);
            var reader = this.Connection.QueryMultiple(sql, data, this.Transaction, this.Timeout);
            return (long)reader.Read().First().Id;
        }


        /// <summary>
        /// Inserts the specified data into the table and returns the automatically incremented ID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns>Auto incremented ID</returns>
        public override async Task<long> InsertAndGetIdAsync<T>(T data, ValuePriority createdAt)
        {
            var query = this.DbProvider.QueryBuilder.Insert<T>(createdAt).Build();
            var sql = ToInsertAndGetIdSql(query.Statement);
            var reader = await this.Connection.QueryMultipleAsync(sql, data, this.Transaction, this.Timeout).ConfigureAwait(false);
            var results = await reader.ReadAsync().ConfigureAwait(false);
            return (long)results.First().Id;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns></returns>
        private string ToInsertAndGetIdSql(string insert)
            =>
$@"{insert};
select cast(scope_identity() as bigint) as Id;";
        #endregion


        #region InsertIgnore
        /// <summary>
        /// Inserts the specified data into the table.
        /// Insertion processing is not performed when there is a collision with a unique constraint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns>Effected row count</returns>
        public override int InsertIgnore<T>(T data, ValuePriority createdAt)
        {
            var query = this.CreateInsertIgnoreQuery<T>(createdAt);
            return this.Connection.Execute(query.Statement, query.BindParameter, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// Inserts the specified data into the table.
        /// Insertion processing is not performed when there is a collision with a unique constraint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns>Effected row count</returns>
        public override Task<int> InsertIgnoreAsync<T>(T data, ValuePriority createdAt)
        {
            var query = this.CreateInsertIgnoreQuery<T>(createdAt);
            return this.Connection.ExecuteAsync(query.Statement, query.BindParameter, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="createdAtPriority"></param>
        /// <returns></returns>
        /// <remarks>
        /// Create SQL like following:
        /// 
        /// merge into [dbo].[Person] as T1
        /// using (select @Age as [Age], @Id as [Id], @Name as [Name]) as T2
        ///     on (T1.[Age] = T2.[Age] and T1.[Name] = T2.[Name]) or (T1.[Id] = T2.[Id])
        /// when not matched then
        ///     insert ([Age], [Id], [Name], [CreatedAt], [ModifiedAt])
        ///     values (@Age, @Id, @Name, @CreatedAt, @CreatedAt);
        /// </remarks>
        private Query CreateInsertIgnoreQuery<T>(ValuePriority createdAtPriority)
        {
            //--- 自動採番列以外に一意制約がない場合は通常の insert 文で OK
            var table = TableInfo.Get<T>(this.DbProvider.Database);
            if (!table.Columns.Any(x => !x.IsAutoIncrement && x.IsUnique))
                return this.DbProvider.QueryBuilder.Insert<T>(createdAtPriority).Build();

            //--- 対象となる列
            var selectColumns
                = table.Columns
                .Where(x => !x.IsCreatedAt)
                .Where(x => !x.IsModifiedAt)
                .ToArray();  // システム定義 (CreatedAt / ModifiedAt) は特別扱いで外す
            var uniqueColumnGroups = selectColumns.ToLookup(x => x.UniqueIndex.Value);
            var insertColumns = table.Columns.Where(x => !x.IsAutoIncrement).ToArray();  // 自動採番列は外す

            //--- 変数ショートカット
            var bracket = this.DbProvider.KeywordBracket;
            var prefix = this.DbProvider.BindParameterPrefix;

            //--- SQL 構築
            var builder = new StringBuilder();
            builder.Append("merge into ");
            builder.Append(table.FullName);
            builder.Append(" as T1");
            builder.Append("using (select ");
            foreach (var x in selectColumns)
            {
                builder.Append(prefix);
                builder.Append(x.MemberName);
                builder.Append(" as ");
                builder.Append(bracket.Begin);
                builder.Append(x.ColumnName);
                builder.Append(bracket.End);
                builder.Append(", ");
            }
            builder.Length -= 2;
            builder.AppendLine(") as T2");
            foreach (var xs in uniqueColumnGroups.WithIndex())
            {
                builder.Append(xs.index == 0 ? "    on " : " or ");
                builder.Append("(");
                foreach (var x in xs.element.WithIndex())
                {
                    if (x.index > 0)
                        builder.Append(" and ");

                    builder.Append("T1.");
                    builder.Append(bracket.Begin);
                    builder.Append(x.element.ColumnName);
                    builder.Append(bracket.End);
                    builder.Append(" = T2.");
                    builder.Append(bracket.Begin);
                    builder.Append(x.element.ColumnName);
                    builder.Append(bracket.End);
                }
                builder.Append(")");
            }
            builder.AppendLine();
            builder.AppendLine("when not matched then");
            builder.Append("    insert (");
            foreach (var x in insertColumns)
            {
                builder.Append(bracket.Begin);
                builder.Append(x.ColumnName);
                builder.Append(bracket.End);
                builder.Append(", ");
            }   
            builder.Length -= 2;
            builder.AppendLine(")");
            builder.Append("    values (");
            foreach (var x in insertColumns)
            {
                if (createdAtPriority == ValuePriority.Default)
                {
                    if (x.IsCreatedAt && x.DefaultValue != null)
                    {
                        builder.Append(x.DefaultValue);
                        builder.Append(", ");
                        continue;
                    }
                    if (x.IsModifiedAt && x.DefaultValue != null)
                    {
                        builder.Append(x.DefaultValue);
                        builder.Append(", ");
                        continue;
                    }
                }
                builder.Append(prefix);
                builder.Append(x.MemberName);
                builder.Append(", ");
            }
            builder.Append(");");

            //--- ok
            return new Query(builder.ToString(), null);
        }
        #endregion


        #region InsertIgnoreMulti
        /// <summary>
        /// Inserts the specified data into the table.
        /// Insertion processing is not performed when there is a collision with a unique constraint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns>Effected row count</returns>
        public override int InsertIgnoreMulti<T>(IEnumerable<T> data, ValuePriority createdAt)
        {
            var query = this.CreateInsertIgnoreQuery<T>(createdAt);
            return this.Connection.Execute(query.Statement, query.BindParameter, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// Inserts the specified data into the table.
        /// Insertion processing is not performed when there is a collision with a unique constraint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns>Effected row count</returns>
        public override Task<int> InsertIgnoreMultiAsync<T>(IEnumerable<T> data, ValuePriority createdAt)
        {
            var query = this.CreateInsertIgnoreQuery<T>(createdAt);
            return this.Connection.ExecuteAsync(query.Statement, query.BindParameter, this.Transaction, this.Timeout);
        }
        #endregion
    }
}
