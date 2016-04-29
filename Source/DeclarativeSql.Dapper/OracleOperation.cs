using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;
using This = DeclarativeSql.Dapper.OracleOperation;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// Oracleデータベースに対する操作を提供します。
    /// </summary>
    internal class OracleOperation : DbOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected OracleOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
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
            => this.CreateBulkInsertCommand(data).ExecuteNonQuery();


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
            => this.CreateBulkInsertCommand(data).ExecuteNonQueryAsync();


        /// <summary>
        /// バルク方式で指定のデータを挿入するためのコマンドを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>コマンド</returns>
        private DbCommand CreateBulkInsertCommand<T>(IEnumerable<T> data)
        {
            //--- 実体化
            data = data.Materialize();

            //--- build DbCommand
            var factory = DbProvider.GetFactory(this.DbKind);
            dynamic command = factory.CreateCommand();
            command.Connection = (dynamic)this.Connection;
            command.CommandText = PrimitiveSql.CreateInsert<T>(this.DbKind, false, true);
            command.BindByName = true;
            command.ArrayBindCount = data.Count();
            if (this.Timeout.HasValue)
                command.CommandTimeout = this.Timeout.Value;

            //--- bind params
            foreach (var x in TableMappingInfo.Create<T>().Columns)
            {
                var getter = AccessorCache<T>.LookupGet(x.PropertyName);
                dynamic parameter = factory.CreateParameter();
                parameter.ParameterName = x.PropertyName;
                parameter.DbType = x.ColumnType;
                parameter.Value = data.Select(y => getter(y)).ToArray();
                command.Parameters.Add(parameter);
            }
            return command;
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// 指定されたレコードをテーブルに挿入し、自動採番IDを返します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>自動採番ID</returns>
        public override long InsertAndGet<T>(T data)
        {
            this.AssertInsertAndGet<T>();
            var param = this.CreateInsertAndGetParameter(data);
            if (param.Item1.ExecuteNonQuery() != 1)
                throw new SystemException("Affected row count is not 1.");
            return Convert.ToInt64(param.Item2.Value);
        }


        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入し、自動採番IDを返します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>自動採番ID</returns>
        public override async Task<long> InsertAndGetAsync<T>(T data)
        {
            this.AssertInsertAndGet<T>();
            var param = this.CreateInsertAndGetParameter(data);
            var result = await param.Item1.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (result != 1)
                throw new SystemException("Affected row count is not 1.");
            return Convert.ToInt64(param.Item2.Value);
        }


        /// <summary>
        /// InsertAndGetするためのパラメーターを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>パラメーター</returns>
        private Tuple<DbCommand, DbParameter> CreateInsertAndGetParameter<T>(T data)
        {
            //--- command
            var factory = DbProvider.GetFactory(this.DbKind);
            dynamic command = factory.CreateCommand();
            command.BindByName = true;
            command.Connection = (dynamic)this.Connection;
            if (this.Timeout.HasValue)
                command.CommandTimeout = this.Timeout.Value;

            //--- parameters
            DbParameter output = null;
            foreach (var x in TableMappingInfo.Create<T>().Columns)
            {
                dynamic parameter = factory.CreateParameter();
                parameter.ParameterName = x.PropertyName;
                parameter.DbType = x.ColumnType;
                if (x.IsPrimaryKey)
                {
                    parameter.Direction = ParameterDirection.Output;
                    output = parameter;
                    command.CommandText =
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)}
returning {x.ColumnName} into :{x.PropertyName}";
                }
                else
                {
                    parameter.Direction = ParameterDirection.Input;
                    parameter.Value = AccessorCache<T>.LookupGet(x.PropertyName)(data);
                }
                command.Parameters.Add(parameter);
            }

            //--- ok
            return Tuple.Create((DbCommand)command, output);
        }
        #endregion
    }
}