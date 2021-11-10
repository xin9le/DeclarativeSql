using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Sql;

namespace DeclarativeSql.DbOperations;



/// <summary>
/// Provides MySQL specific database operation.
/// </summary>
internal class MySqlOperation : DbOperation
{
    #region Constructors
    /// <inheritdoc/>
    public MySqlOperation(IDbConnection connection, IDbTransaction? transaction, DbProvider provider, int? timeout)
        : base(connection, transaction, provider, timeout)
    { }
    #endregion


    #region InsertAndGetId
    /// <inheritdoc/>
    public override long InsertAndGetId<T>(T data, ValuePriority createdAt)
    {
        var sql = this.CreateInsertAndGetIdSql<T>(createdAt);
        var reader = this.Connection.QueryMultiple(sql, data, this.Transaction, this.Timeout);
        return (long)reader.Read().First().Id;
    }


    /// <inheritdoc/>
    public override async Task<long> InsertAndGetIdAsync<T>(T data, ValuePriority createdAt, CancellationToken cancellationToken)
    {
        var sql = this.CreateInsertAndGetIdSql<T>(createdAt);
        var command = new CommandDefinition(sql, data, this.Transaction, this.Timeout, null, CommandFlags.Buffered, cancellationToken);
        var reader = await this.Connection.QueryMultipleAsync(command).ConfigureAwait(false);
        var results = await reader.ReadAsync().ConfigureAwait(false);
        return (long)results.First().Id;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="createdAtPriority"></param>
    /// <returns></returns>
    private string CreateInsertAndGetIdSql<T>(ValuePriority createdAtPriority)
    {
        using (var builder = new QueryBuilder<T>(this.DbProvider))
        {
            builder.Insert(createdAtPriority);
            builder.AppendLine(';');
            builder.Append("select last_insert_id() as Id;");
            var query = builder.Build();
            return query.Statement;
        }
    }
    #endregion


    #region InsertIgnore
    /// <inheritdoc/>
    public override int InsertIgnore<T>(T data, ValuePriority createdAt)
    {
        var sql = this.CreateInsertIgnoreSql<T>(createdAt);
        return this.Connection.Execute(sql, data, this.Transaction, this.Timeout);
    }


    /// <inheritdoc/>
    public override Task<int> InsertIgnoreAsync<T>(T data, ValuePriority createdAt, CancellationToken cancellationToken)
    {
        var sql = this.CreateInsertIgnoreSql<T>(createdAt);
        var command = new CommandDefinition(sql, data, this.Transaction, this.Timeout, null, CommandFlags.Buffered, cancellationToken);
        return this.Connection.ExecuteAsync(command);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="createdAt"></param>
    /// <returns></returns>
    private string CreateInsertIgnoreSql<T>(ValuePriority createdAt)
    {
        var query = QueryBuilder.Insert<T>(this.DbProvider, createdAt);
        return query.Statement.Replace("insert into", "insert ignore into");
    }
    #endregion


    #region InsertIgnoreMulti
    /// <inheritdoc/>
    public override int InsertIgnoreMulti<T>(IEnumerable<T> data, ValuePriority createdAt)
    {
        var sql = this.CreateInsertIgnoreSql<T>(createdAt);
        return this.Connection.Execute(sql, data, this.Transaction, this.Timeout);
    }


    /// <inheritdoc/>
    public override Task<int> InsertIgnoreMultiAsync<T>(IEnumerable<T> data, ValuePriority createdAt, CancellationToken cancellationToken)
    {
        var sql = this.CreateInsertIgnoreSql<T>(createdAt);
        var command = new CommandDefinition(sql, data, this.Transaction, this.Timeout, null, CommandFlags.Buffered, cancellationToken);
        return this.Connection.ExecuteAsync(command);
    }
    #endregion
}
