using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Sql;



namespace DeclarativeSql.DbOperations
{
    /// <summary>
    /// Provides SQLite specific database operation.
    /// </summary>
    internal class SqliteOperation : DbOperation
    {
        #region Constructors
        /// <inheritdoc/>
        public SqliteOperation(IDbConnection connection, IDbTransaction? transaction, DbProvider provider, int? timeout)
            : base(connection, transaction, provider, timeout)
        { }
        #endregion


        #region InsertAndGetId
        /// <inheritdoc/>
        public override long InsertAndGetId<T>(T data, ValuePriority createdAt)
        {
            var sql = this.CreateInsertAndGetIdSql<T>(createdAt);
            var reader = this.Connection.QueryMultiple(sql, data, this.Transaction, this.Timeout);
            return (long)reader.Read().First().Id;
        }


        /// <inheritdoc/>
        public override async Task<long> InsertAndGetIdAsync<T>(T data, ValuePriority createdAt, CancellationToken cancellationToken)
        {
            var sql = this.CreateInsertAndGetIdSql<T>(createdAt);
            var command = new CommandDefinition(sql, data, this.Transaction, this.Timeout, null, CommandFlags.Buffered, cancellationToken);
            var reader = await this.Connection.QueryMultipleAsync(command).ConfigureAwait(false);
            var results = await reader.ReadAsync().ConfigureAwait(false);
            return (long)results.First().Id;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createdAtPriority"></param>
        /// <returns></returns>
        private string CreateInsertAndGetIdSql<T>(ValuePriority createdAtPriority)
        {
            using (var builder = new QueryBuilder<T>(this.DbProvider))
            {
                builder.Insert(createdAtPriority);
                builder.AppendLine(';');
                builder.Append("select last_insert_rowid() as Id;");
                var query = builder.Build();
                return query.Statement;
            }
        }
        #endregion
    }
}
