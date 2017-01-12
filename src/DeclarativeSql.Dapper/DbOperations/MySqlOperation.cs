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
using This = DeclarativeSql.Dapper.MySqlOperation;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// Provides the database operations for MySQL.
    /// </summary>
    internal class MySqlOperation : DbOperation
    {
        #region Properties
        /// <summary>
        /// Gets the delegate that escapes the specified string.
        /// </summary>
        private static Func<string, string> Escape { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instanse.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="provider">Database provider</param>
        /// <param name="timeout">Timeout</param>
        protected MySqlOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
            : base(connection, transaction, provider, timeout)
        {}


        /// <summary>
        /// Calls when first access.
        /// </summary>
        static MySqlOperation()
        {
            var name = new AssemblyName(DbProvider.MySql.AssemblyName);
            var assembly = Assembly.Load(name);
            var type = assembly.GetType("MySql.Data.MySqlClient.MySqlHelper").GetType();
            var method = type.GetRuntimeMethod("EscapeString", new []{ typeof(string) });
            This.Escape = (Func<string, string>)method.CreateDelegate(typeof(Func<string, string>));
        }
        #endregion


        #region BulkInsert
        /// <summary>
        /// Inserts the specified record into the table by bulk method.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Affected row count.</returns>
        public override int BulkInsert<T>(IEnumerable<T> data)
        {
            var sql = this.CreateBulkInsertSql(data);
            return this.Connection.Execute(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// Asynchronously inserts the specified record into the table by bulk method.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Affected row count.</returns>
        public override Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            var sql = this.CreateBulkInsertSql(data);
            return this.Connection.ExecuteAsync(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// Generates SQL statement for bulk inserts from the specified data.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>SQL</returns>
        private string CreateBulkInsertSql<T>(IEnumerable<T> data)
        {
            var prefix = this.DbProvider.BindParameterPrefix;
            var table  = TableMappingInfo.Create<T>();
            var columnNames = table.Columns.Select(x => "    " + x.ColumnName(this.DbProvider.KeywordBrackets));
            var builder = new StringBuilder();
            builder.AppendLine($"insert into {table.FullName(this.DbProvider.KeywordBrackets)}");
            builder.AppendLine("(");
            builder.AppendLine(string.Join($",{Environment.NewLine}", columnNames));
            builder.AppendLine(")");
            builder.Append("values");

            var getters = table.Columns.Select(c => AccessorCache<T>.LookupGet(c.PropertyName)).ToArray();
            foreach (var x in data)
            {
                builder.AppendLine();
                builder.Append("(");
                var values = getters.Select(f => This.ToSqlLiteral(f(x)));
                builder.Append(string.Join(", ", values));
                builder.Append("),");
            }
            builder.Length--;  //--- 最後の「,」を削除

            return builder.ToString();
        }


        /// <summary>
        /// Converts the specified value to literal for SQL.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Literal for SQL</returns>
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
        /// Generates SQL to insert record and get the automatically assigned ID.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <returns>SQL</returns>
        protected override string CreateInsertAndGetSql<T>()
            =>
$@"{this.DbProvider.Insert<T>().ToString()};
select last_insert_id() as Id;";
        #endregion
    }
}