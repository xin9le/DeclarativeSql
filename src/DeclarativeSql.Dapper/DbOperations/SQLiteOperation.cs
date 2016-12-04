using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;



namespace DeclarativeSql.Dapper
{
    /*
    /// <summary>
    /// SQLiteデータベースに対する操作を提供します。
    /// </summary>
    internal class SQLiteOperation : DbOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instanse.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="provider">Database provider</param>
        /// <param name="timeout">Timeout</param>
        protected SQLiteOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
            : base(connection, transaction, provider, timeout)
        {}
        #endregion


        #region BulkInsert
        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override int BulkInsert<T>(IEnumerable<T> data)
        {
            //--- 挿入処理本体
            Func<IEnumerable<T>, IDbTransaction, int> insert = (collection, transaction) =>
            {
                var result = 0;
                var sql = PrimitiveSql.CreateInsert<T>(this.DbKind, false, true);
                foreach (var x in collection)
                {
                    var value = this.Connection.Execute(sql, x, transaction, this.Timeout);
                    Interlocked.Add(ref result, value);
                }
                return result;
            };

            //--- トランザクションが外部から指定されている場合はそれを利用
            if (this.Transaction != null)
                return insert(data, this.Transaction);

            //--- トランザクションが外部から指定されていない場合は新規に作成
            //--- SQLiteにおけるバルクインサートの魔法
            using (var transaction = this.Connection.StartTransaction())
            {
                var result = insert(data, transaction.Raw);
                transaction.Complete();
                return result;
            }
        }


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override async Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            //--- 挿入処理本体
            Func<IEnumerable<T>, IDbTransaction, Task<int>> insert = async (collection, transaction) =>
            {
                var result = 0;
                var sql = PrimitiveSql.CreateInsert<T>(this.DbKind, false, true);
                foreach (var x in collection)
                {
                    var value = await this.Connection.ExecuteAsync(sql, x, transaction, this.Timeout).ConfigureAwait(false);
                    Interlocked.Add(ref result, value);
                }
                return result;
            };

            //--- トランザクションが外部から指定されている場合はそれを利用
            if (this.Transaction != null)
                return await insert(data, this.Transaction).ConfigureAwait(false);

            //--- トランザクションが外部から指定されていない場合は新規に作成
            //--- SQLiteにおけるバルクインサートの魔法
            using (var transaction = this.Connection.StartTransaction())
            {
                var result = await insert(data, transaction.Raw).ConfigureAwait(false);
                transaction.Complete();
                return result;
            }
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected override string CreateInsertAndGetSql<T>()
            =>
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)};
select last_insert_rowid() as Id;";
        #endregion
    }
    */
}