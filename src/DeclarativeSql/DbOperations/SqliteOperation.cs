using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Text;
using Dapper;



namespace DeclarativeSql.DbOperations
{
    /// <summary>
    /// Provides SQLite specific database operation.
    /// </summary>
    internal class SqliteOperation : DbOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="provider"></param>
        /// <param name="timeout"></param>
        public SqliteOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
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
        /// <returns></returns>
        private string ToInsertAndGetIdSql(string insert)
            => ZString.Concat(insert, ';', Environment.NewLine, "select last_insert_rowid() as Id;");
        #endregion
    }
}
