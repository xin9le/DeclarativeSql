using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;
using FastMember;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// Provides the database operations for SQL Server.
    /// </summary>
    internal class SqlServerOperation : DbOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instanse.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="provider">Database provider</param>
        /// <param name="timeout">Timeout</param>
        protected SqlServerOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
            : base(connection, transaction, provider, timeout)
        {}
        #endregion


        #region BulkInsert (using DbDataReader)
        /// <summary>
        /// Inserts the specified record into the table by bulk method.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Affected row count.</returns>
        public override int BulkInsert<T>(IEnumerable<T> data)
        {
            using (var executor = this.CreateBulkExecutor())
            {
                data = data.Materialize();
                var param = this.SetupBulkInsert(executor, data);
                executor.WriteToServer(param);
                return data.Count();
            }
        }


        /// <summary>
        /// Asynchronously inserts the specified record into the table by bulk method.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Affected row count.</returns>
        public override async Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            using (var executor = this.CreateBulkExecutor())
            {
                data = data.Materialize();
                var param = this.SetupBulkInsert(executor, data);
                await executor.WriteToServerAsync(param).ConfigureAwait(false);
                return data.Count();
            }
        }


        /// <summary>
        /// Generates bulk process executor.
        /// </summary>
        /// <returns>Instance</returns>
        private SqlBulkCopy CreateBulkExecutor()
            => new SqlBulkCopy(this.Connection as SqlConnection, SqlBulkCopyOptions.Default, this.Transaction as SqlTransaction);


        /// <summary>
        /// Prepares for bulk insertion processing.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="executor">Bulk executor.</param>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Data reader</returns>
        private DbDataReader SetupBulkInsert<T>(SqlBulkCopy executor, IEnumerable<T> data)
        {
            //--- タイムアウト
            if (this.Timeout.HasValue)
                executor.BulkCopyTimeout = this.Timeout.Value;
            
            //--- 対象テーブル名
            var table = TableMappingInfo.Create<T>();
            executor.DestinationTableName = table.FullName(this.DbProvider.KeywordBrackets);

            //--- データ読み込みをラップ
            var propertyNames = table.Columns.Select(x => x.PropertyName).ToArray();
            return ObjectReader.Create(data, propertyNames);
        }
        #endregion


        #region BulkInsert (using DataTable)
        /*
        /// <summary>
        /// Inserts the specified record into the table by bulk method.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Affected row count.</returns>
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
        /// Asynchronously inserts the specified record into the table by bulk method.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Affected row count.</returns>
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
        /// Generates bulk process executor.
        /// </summary>
        /// <returns>Instance</returns>
        private SqlBulkCopy CreateBulkExecutor()
            => new SqlBulkCopy(this.Connection as SqlConnection, SqlBulkCopyOptions.Default, this.Transaction as SqlTransaction);


        /// <summary>
        /// Prepares for bulk insertion processing.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="executor">Bulk executor.</param>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Data table</returns>
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
                table.Columns.Add(new DataColumn
                {
                    ColumnName = x.PropertyName,
                    DataType = x.IsNullable ? Nullable.GetUnderlyingType(x.PropertyType) : x.PropertyType,
                    AllowDBNull = x.IsNullable,
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
        */
        #endregion


        #region InsertAndGet
        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected override string CreateInsertAndGetSql<T>()
            =>
$@"{this.DbProvider.Sql.CreateInsert<T>()};
select cast(scope_identity() as bigint) as Id;";
        #endregion
    }
}