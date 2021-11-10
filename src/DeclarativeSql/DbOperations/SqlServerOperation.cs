using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;
using DeclarativeSql.Sql;

namespace DeclarativeSql.DbOperations;



/// <summary>
/// Provides SQL Server specific database operation.
/// </summary>
internal class SqlServerOperation : DbOperation
{
    #region Constructors
    /// <inheritdoc/>
    public SqlServerOperation(IDbConnection connection, IDbTransaction? transaction, DbProvider provider, int? timeout)
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
            builder.Append("select cast(scope_identity() as bigint) as Id;");
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
    /// <param name="createdAtPriority"></param>
    /// <returns></returns>
    /// <remarks>
    /// Create SQL like following:
    /// 
    /// merge into [dbo].[Person] as T1
    /// using (select @Age as [Age], @Id as [Id], @Name as [Name]) as T2
    ///     on (T1.[Age] = T2.[Age] and T1.[Name] = T2.[Name]) or (T1.[Id] = T2.[Id])
    /// when not matched then
    ///     insert ([Age], [Id], [Name], [CreatedAt], [ModifiedAt])
    ///     values (@Age, @Id, @Name, @CreatedAt, @CreatedAt);
    /// </remarks>
    private string CreateInsertIgnoreSql<T>(ValuePriority createdAtPriority)
    {
        //--- 自動採番列以外に一意制約がない場合は通常の insert 文で OK
        var table = TableInfo.Get<T>(this.DbProvider.Database);
        if (!table.Columns.Any(x => !x.IsAutoIncrement && x.IsUnique))
            return QueryBuilder.Insert<T>(this.DbProvider, createdAtPriority).Statement;

        //--- 対象となる列
        var selectColumns
            = table.Columns
            .Where(x => !x.IsCreatedAt)
            .Where(x => !x.IsModifiedAt)
            .ToArray();  // システム定義 (CreatedAt / ModifiedAt) は特別扱いで外す
        var uniqueColumnGroups = selectColumns.ToLookup(x => x.UniqueIndex);
        var insertColumns = table.Columns.Where(x => !x.IsAutoIncrement).ToArray();  // 自動採番列は外す

        //--- 変数ショートカット
        var bracket = this.DbProvider.KeywordBracket;
        var prefix = this.DbProvider.BindParameterPrefix;

        //--- SQL 構築
        var builder = new StringBuilder();
        builder.Append("merge into ");
        builder.Append(table.FullName);
        builder.Append(" as T1");
        builder.Append("using (select ");
        foreach (var x in selectColumns)
        {
            builder.Append(prefix);
            builder.Append(x.MemberName);
            builder.Append(" as ");
            builder.Append(bracket.Begin);
            builder.Append(x.ColumnName);
            builder.Append(bracket.End);
            builder.Append(", ");
        }
        builder.Length -= 2;
        builder.AppendLine(") as T2");
        foreach (var xs in uniqueColumnGroups.WithIndex())
        {
            builder.Append(xs.index == 0 ? "    on " : " or ");
            builder.Append('(');
            foreach (var x in xs.element.WithIndex())
            {
                if (x.index > 0)
                    builder.Append(" and ");

                builder.Append("T1.");
                builder.Append(bracket.Begin);
                builder.Append(x.element.ColumnName);
                builder.Append(bracket.End);
                builder.Append(" = T2.");
                builder.Append(bracket.Begin);
                builder.Append(x.element.ColumnName);
                builder.Append(bracket.End);
            }
            builder.Append(')');
        }
        builder.AppendLine();
        builder.AppendLine("when not matched then");
        builder.Append("    insert (");
        foreach (var x in insertColumns)
        {
            builder.Append(bracket.Begin);
            builder.Append(x.ColumnName);
            builder.Append(bracket.End);
            builder.Append(", ");
        }
        builder.Length -= 2;
        builder.AppendLine(")");
        builder.Append("    values (");
        foreach (var x in insertColumns)
        {
            if (createdAtPriority == ValuePriority.Default)
            {
                if (x.IsCreatedAt && x.DefaultValue is not null)
                {
                    builder.Append(x.DefaultValue);
                    builder.Append(", ");
                    continue;
                }
                if (x.IsModifiedAt && x.DefaultValue is not null)
                {
                    builder.Append(x.DefaultValue);
                    builder.Append(", ");
                    continue;
                }
            }
            builder.Append(prefix);
            builder.Append(x.MemberName);
            builder.Append(", ");
        }
        builder.Append(");");

        //--- ok
        return builder.ToString();
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
