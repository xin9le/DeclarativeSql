using System.Data;



namespace DeclarativeSql.Dapper
{
    /*
    /// <summary>
    /// SQL Server Compactデータベースに対する操作を提供します。
    /// </summary>
    internal class SqlServerCeOperation : DbOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instanse.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="provider">Database provider</param>
        /// <param name="timeout">Timeout</param>
        protected SqlServerCeOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
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
            =>
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)};
select @@IDENTITY as Id;";
        #endregion
    }
    */
}