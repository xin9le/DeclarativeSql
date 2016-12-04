using System.Data;
using System.Linq;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Dapper
{
    /*
    /// <summary>
    /// PostgreSqlデータベースに対する操作を提供します。
    /// </summary>
    internal class PostgreSqlOperation : DbOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instanse.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="provider">Database provider</param>
        /// <param name="timeout">Timeout</param>
        protected PostgreSqlOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
            : base(connection, transaction, provider, timeout)
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
    */
}