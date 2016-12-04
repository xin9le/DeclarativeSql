using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// Sql Serverデータベースに対する操作を提供します。
    /// </summary>
    internal class SqlServerOperation : DbOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected SqlServerOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
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
            using (var executor = this.CreateBulkExecutor())
            {
                var param = this.SetupBulkInsert(executor, data);
                executor.WriteToServer(param);
                return param.Rows.Count;
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
            using (var executor = this.CreateBulkExecutor())
            {
                var param = this.SetupBulkInsert(executor, data);
                await executor.WriteToServerAsync(param).ConfigureAwait(false);
                return param.Rows.Count;
            }
        }


        /// <summary>
        /// バルク処理の実行機能を生成します。
        /// </summary>
        /// <returns>インスタンス</returns>
        private SqlBulkCopy CreateBulkExecutor() => new SqlBulkCopy(this.Connection as SqlConnection, SqlBulkCopyOptions.Default, this.Transaction as SqlTransaction);


        /// <summary>
        /// バルク方式での挿入処理の準備を行います。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="executor">バルク処理実行機能</param>
        /// <param name="data">挿入する生データ</param>
        /// <returns>挿入するデータ</returns>
        private DataTable SetupBulkInsert<T>(SqlBulkCopy executor, IEnumerable<T> data)
        {
            //--- タイムアウト
            if (this.Timeout.HasValue)
                executor.BulkCopyTimeout = this.Timeout.Value;
            
            //--- 対象テーブル名
            var info = TableMappingInfo.Create<T>();
            executor.DestinationTableName = info.FullName;

            //--- 列のマップ
            var table = new DataTable();
            var getters = new List<Func<T, object>>();
            foreach (var x in info.Columns)
            {
                executor.ColumnMappings.Add(x.PropertyName, x.ColumnName);
                table.Columns.Add(new DataColumn {
                    ColumnName = x.PropertyName,
                    DataType = x.IsNullable ? Nullable.GetUnderlyingType(x.PropertyType) : x.PropertyType,
                    AllowDBNull = x.IsNullable
                });
                getters.Add(AccessorCache<T>.LookupGet(x.PropertyName));
            }

            //--- データ生成
            foreach (var x in data)
            {
                var row = table.NewRow();
                for (int i = 0; i < getters.Count; i++)
                    row[i] = getters[i](x);
                table.Rows.Add(row);
            }
            return table;
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
select cast(scope_identity() as bigint) as Id;";
        #endregion
    }
}