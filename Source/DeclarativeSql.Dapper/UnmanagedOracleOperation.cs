using System.Data;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// UnmanagedなODP.NETによるOracleデータベースに対する操作を提供します。
    /// </summary>
    internal class UnmanagedOracleOperation : OracleOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected UnmanagedOracleOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion
    }
}