using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;

namespace DeclarativeSql.Tests.Cases;



public class ThenByTest
{
    private DbProvider DbProvider { get; } = DbProvider.SqlServer;


    [Fact]
    public void Ascending()
    {
        var actual = CreateActualQuery();
        var expect =
@"order by
    [名前],
    [CreatedAt]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();

        Query CreateActualQuery()
        {
            using (var builder = new QueryBuilder<Person>(this.DbProvider))
            {
                builder.OrderBy(x => x.Name);
                builder.ThenBy(x => x.CreatedAt);
                return builder.Build();
            }
        }
    }


    [Fact]
    public void Descending()
    {
        var actual = CreateActualQuery();
        var expect =
@"order by
    [Age] desc,
    [ModifiedAt] desc";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();

        Query CreateActualQuery()
        {
            using (var builder = new QueryBuilder<Person>(this.DbProvider))
            {
                builder.OrderByDescending(x => x.Age);
                builder.ThenByDescending(x => x.ModifiedAt);
                return builder.Build();
            }
        }
    }
}
