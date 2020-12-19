using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;
using FastMember;
using Microsoft.Data.SqlClient;



namespace DeclarativeSql.DbOperations
{
    /// <summary>
    /// Provides Microsoft.Data.SqlClient specific database operation.
    /// </summary>
    internal class MicrosoftSqlClientOperation : SqlServerOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="provider"></param>
        /// <param name="timeout"></param>
        private MicrosoftSqlClientOperation(IDbConnection connection, IDbTransaction? transaction, DbProvider provider, int? timeout)
            : base(connection, transaction, provider, timeout)
        { }


        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static DbOperation Create(IDbConnection connection, IDbTransaction? transaction, int? timeout)
            => new MicrosoftSqlClientOperation(connection, transaction, DbProvider.SqlServer, timeout);
        #endregion


        #region BulkInsert
        /// <summary>
        /// Inserts the specified data into the table using the bulk method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns>Effected rows count</returns>
        public override int BulkInsert<T>(IEnumerable<T> data, ValuePriority createdAt)
        {
            using var executor = new SqlBulkCopy(this.Connection as SqlConnection, SqlBulkCopyOptions.Default, this.Transaction as SqlTransaction);
            data = data.Materialize();
            var param = this.SetupBulkInsert(executor, data, createdAt);
            executor.WriteToServer(param);
            return data.Count();
        }


        /// <summary>
        /// Inserts the specified data into the table using the bulk method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Effected rows count</returns>
        public override async Task<int> BulkInsertAsync<T>(IEnumerable<T> data, ValuePriority createdAt, CancellationToken cancellationToken = default)
        {
            using var executor = new SqlBulkCopy(this.Connection as SqlConnection, SqlBulkCopyOptions.Default, this.Transaction as SqlTransaction);
            data = data.Materialize();
            var param = this.SetupBulkInsert(executor, data, createdAt);
            await executor.WriteToServerAsync(param, cancellationToken).ConfigureAwait(false);
            return data.Count();
        }


        /// <summary>
        /// Prepares for bulk insertion processing.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="executor">Bulk executor.</param>
        /// <param name="data">Inserting target data.</param>
        /// <param name="createdAt"></param>
        /// <returns>Data reader</returns>
        private DataTable SetupBulkInsert<T>(SqlBulkCopy executor, IEnumerable<T> data, ValuePriority createdAt)
        {
            //--- Timeout -> CommandTimeout -> BulkCopyTimeout
            executor.BulkCopyTimeout = this.Timeout ?? SqlMapper.Settings.CommandTimeout ?? executor.BulkCopyTimeout;

            //--- Target table
            var tableMappings = TableInfo.Get<T>(this.DbProvider.Database);
            executor.DestinationTableName = tableMappings.FullName;

            //--- Extract mapping columns
            var columnMappings
                = tableMappings.Columns
                .Where(x => !x.IsAutoIncrement)
                .Where(x =>
                {
                    if (createdAt == ValuePriority.Default)
                    {
                        if ((x.IsCreatedAt && x.DefaultValue is not null)
                        || (x.IsModifiedAt && x.DefaultValue is not null))
                            return false;
                    }
                    return true;
                })
                .ToArray();

            //--- Setup columns
            var columns
                = columnMappings
                .Select(x =>
                {
                    var isNullable = x.MemberType.IsNullable();
                    return new DataColumn
                    {
                        ColumnName = x.ColumnName,
                        AllowDBNull = isNullable || x.AllowNull,
                        DataType = isNullable ? Nullable.GetUnderlyingType(x.MemberType)! : x.MemberType,
                    };
                });
            var table = new DataTable();
            foreach (var x in columns)
            {
                executor.ColumnMappings.Add(x.ColumnName, x.ColumnName);
                table.Columns.Add(x);
            }

            //--- Setup rows
            foreach (var x in data)
            {
                var row = table.NewRow();
                var accessor = ObjectAccessor.Create(x);
                foreach (var y in columnMappings)
                    row[y.ColumnName] = accessor[y.MemberName] ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        #endregion
    }
}
