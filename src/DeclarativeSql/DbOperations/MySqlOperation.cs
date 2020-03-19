using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Text;
using Dapper;
using DeclarativeSql.Sql;



namespace DeclarativeSql.DbOperations
{
    /// <summary>
    /// Provides MySQL specific database operation.
    /// </summary>
    internal class MySqlOperation : DbOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="provider"></param>
        /// <param name="timeout"></param>
        public MySqlOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
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
            var query = QueryBuilder.Insert<T>(this.DbProvider, createdAt);
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
            var query = QueryBuilder.Insert<T>(this.DbProvider, createdAt);
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
            => ZString.Concat(insert, ';', Environment.NewLine, "select last_insert_id() as Id;");
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
        /// <param name="createdAt"></param>
        /// <returns></returns>
        private Query CreateInsertIgnoreQuery<T>(ValuePriority createdAt)
        {
            var query = QueryBuilder.Insert<T>(this.DbProvider, createdAt);
            var sql = query.Statement.Replace("insert into", "insert ignore into");
            return new Query(sql, query.BindParameter);
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
