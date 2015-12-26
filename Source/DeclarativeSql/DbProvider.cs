using System;
using System.Data;
using System.Data.Common;
using This = DeclarativeSql.DbProvider;



namespace DeclarativeSql
{
    /// <summary>
    /// データベースに関する機能を提供します。
    /// </summary>
    public static class DbProvider
    {
        #region メソッド
        /// <summary>
        /// 指定されたデータベースの種類に応じたデータベース機能を提供します。
        /// </summary>
        /// <param name="dbKind">データベースの種類</param>
        /// <returns>データベース機能</returns>
        public static DbProviderFactory GetFactory(DbKind dbKind)
        {
            var name = dbKind.GetProviderName();
            return DbProviderFactories.GetFactory(name);
        }


        /// <summary>
        /// 指定されたデータベースに対する接続管理機能を生成します。
        /// </summary>
        /// <param name="dbKind">データーベースの種類</param>
        /// <returns>データーベース接続</returns>
        public static IDbConnection CreateConnection(this DbKind dbKind) => This.GetFactory(dbKind).CreateConnection();


        /// <summary>
        /// 指定されたデータベースに対する接続管理機能を生成します。
        /// </summary>
        /// <param name="dbKind">データーベースの種類</param>
        /// <param name="connectionString">接続文字列</param>
        /// <returns>データーベース接続</returns>
        public static IDbConnection CreateConnection(this DbKind dbKind, string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var connection = dbKind.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }


        /// <summary>
        /// 指定されたデータベーストランザクションをスコープ管理可能なデータベーストランザクションに変換します。
        /// </summary>
        /// <param name="transaction">対象となるトランザクション</param>
        /// <returns>生成されたトランザクションインスタンス</returns>
        public static ScopeTransaction Wrap(this IDbTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return new ScopeTransaction(transaction);
        }


        /// <summary>
        /// 指定されたデータベース接続に対してトランザクションを開始します。
        /// </summary>
        /// <param name="connection">トランザクションを開始するデータベース接続</param>
        /// <returns>トランザクション</returns>
        public static ScopeTransaction StartTransaction(this IDbConnection connection) => connection.StartTransactionCore(null);


        /// <summary>
        /// 指定されたデータベース接続に対して、指定の分離レベルのトランザクションを開始します。
        /// </summary>
        /// <param name="connection">トランザクションを開始するデータベース接続</param>
        /// <param name="isolationLevel">分離レベル</param>
        /// <returns>トランザクション</returns>
        public static ScopeTransaction StartTransaction(this IDbConnection connection, IsolationLevel isolationLevel) => connection.StartTransactionCore(isolationLevel);


        /// <summary>
        /// 指定されたデータベース接続に対して、指定の分離レベルのトランザクションを開始します。
        /// </summary>
        /// <param name="connection">トランザクションを開始するデータベース接続</param>
        /// <param name="isolationLevel">分離レベル</param>
        /// <returns>トランザクション</returns>
        private static ScopeTransaction StartTransactionCore(this IDbConnection connection, IsolationLevel? isolationLevel)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var transaction = isolationLevel.HasValue
                            ? connection.BeginTransaction(isolationLevel.Value)
                            : connection.BeginTransaction();
            return transaction.Wrap();
        }
        #endregion
    }
}