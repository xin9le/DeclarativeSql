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
        public static IDbConnection CreateConnection(this DbKind dbKind)
        {
            return This.GetFactory(dbKind).CreateConnection();
        }


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
        #endregion
    }
}