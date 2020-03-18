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
            var actual = QueryBuilder.OrderBy<Person>(x => x.Name).Build(this.DbProvider);
            var expect =
@"order by
    [名前]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void Descending()
        {
            var actual = QueryBuilder.OrderByDescending<Person>(x => x.Age).Build(this.DbProvider);
            var expect =
@"order by
    [Age] desc";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }
    }
}
