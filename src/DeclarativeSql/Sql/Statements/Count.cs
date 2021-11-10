using Cysharp.Text;
using DeclarativeSql.Mapping;

namespace DeclarativeSql.Sql.Statements;



/// <summary>
/// Represents count statement.
/// </summary>
/// <typeparam name="T"></typeparam>
internal readonly struct Count<T> : ISql
{
    #region ISql implementations
    /// <inheritdoc/>
    public void Build(DbProvider dbProvider, TableInfo table, ref Utf16ValueStringBuilder builder, ref BindParameter? bindParameter)
    {
        var bracket = dbProvider.KeywordBracket;
        builder.Append("select count(*) as ");
        builder.Append(bracket.Begin);
        builder.Append("Count");
        builder.Append(bracket.End);
        builder.Append(" from ");
        builder.Append(table.FullName);
    }
    #endregion
}
