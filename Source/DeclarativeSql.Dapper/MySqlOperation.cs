using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// MySqlデータベースに対する操作を提供します。
    /// </summary>
    internal class MySqlOperation : DbOperation
    {
        #region プロパティ
        /// <summary>
        /// 指定された文字列をエスケープするデリゲートを取得します。
        /// </summary>
        private static Func<string, string> Escape { get; } = (Func<string, string>)Delegate.CreateDelegate
        (
            typeof(Func<string, string>),
            AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(x => x.GetType("MySql.Data.MySqlClient.MySqlHelper"))
                .First(x => x != null)
                .GetRuntimeMethod("EscapeString", new []{ typeof(string) })
        );
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected MySqlOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
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
            var sql = this.CreateBulkInsertSql(data);
            return this.Connection.Execute(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            var sql = this.CreateBulkInsertSql(data);
            return this.Connection.ExecuteAsync(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定したデータからバルクインサート用のSQL文を生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>SQL文</returns>
        private string CreateBulkInsertSql<T>(IEnumerable<T> data)
        {
            var prefix  = this.DbKind.GetBindParameterPrefix();
            var table   = TableMappingInfo.Create<T>();
            var columnNames = table.Columns.Select(x => "    " + x.ColumnName);
            var builder = new StringBuilder();
            builder.AppendLine($"insert into {table.FullName(this.DbKind)}");
            builder.AppendLine("(");
            builder.AppendLine(string.Join($",{Environment.NewLine}", columnNames));
            builder.AppendLine(")");
            builder.Append("values");

            var getters = table.Columns.Select(c => AccessorCache<T>.LookupGet(c.PropertyName)).ToArray();
            foreach (var x in data)
            {
                builder.AppendLine();
                builder.Append("(");
                var values = getters.Select(f => ToSqlLiteral(f(x)));
                builder.Append(string.Join(", ", values));
                builder.Append("),");
            }
            builder.Length--;  //--- 最後の「,」を削除

            return builder.ToString();
        }


        /// <summary>
        /// 指定された値をSQL用の文字に変換します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>SQL用の文字列</returns>
        private static string ToSqlLiteral(object value)
        {
            if (value == null)      return "NULL";
            if (value is string)    return $"'{Escape(value.ToString())}'";
            if (value is bool)      return Convert.ToInt32(value).ToString();
            if (value is Enum)      return ((Enum)value).ToString("d");
            if (value is DateTime)  return $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'";
            if (value is TimeSpan)  return $"'{((TimeSpan)value).ToString("HH:mm:ss")}'";
            if (value is Guid)      return $"'{value.ToString()}'";
            return Escape(value.ToString());
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
select last_insert_id() as Id;";
        #endregion
    }
}