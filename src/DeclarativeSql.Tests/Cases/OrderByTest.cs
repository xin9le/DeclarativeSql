using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;

namespace DeclarativeSql.Tests.Cases;



public class OrderByTest
{
    private DbProvider DbProvider { get; } = DbProvider.SqlServer;


    [Fact]
    public void Ascending()
    {
        var actual = QueryBuilder.OrderBy<Person>(this.DbProvider, x => x.Name);
        var expect =
@"order by
    [名前]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }


    [Fact]
    public void Descending()
    {
        var actual = QueryBuilder.OrderByDescending<Person>(this.DbProvider, x => x.Age);
        var expect =
@"order by
    [Age] desc";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }
}
