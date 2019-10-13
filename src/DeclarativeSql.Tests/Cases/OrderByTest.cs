using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class OrderByTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void Ascending()
        {
            var actual = this.DbProvider.QueryBuilder.OrderBy<Person>(x => x.Name).Build();
            var expect =
@"order by
    [名前]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void Descending()
        {
            var actual = this.DbProvider.QueryBuilder.OrderByDescending<Person>(x => x.Age).Build();
            var expect =
@"order by
    [Age] desc";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }
    }
}
