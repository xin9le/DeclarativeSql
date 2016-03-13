using System.Data;
using System.Linq;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// PostgreSqlデータベースに対する操作を提供します。
    /// </summary>
    internal class PostgreSqlOperation : DbOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected PostgreSqlOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion


        #region InsertAndGet
        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected override string CreateInsertAndGetSql<T>()
        {
            var sequence = TableMappingInfo.Create<T>().Columns.First(x => x.IsPrimaryKey).Sequence;
            return
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)};
select currval({sequence.FullName}) as Id;";
        }
        #endregion
    }
}