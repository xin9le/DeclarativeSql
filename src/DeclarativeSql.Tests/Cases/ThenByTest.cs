using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class ThenByTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void Ascending()
        {
            var actual
                = QueryBuilder
                .OrderBy<Person>(x => x.Name)
                .ThenBy(x => x.CreatedAt)
                .Build(this.DbProvider);
            var expect =
@"order by
    [名前],
    [CreatedAt]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void Descending()
        {
            var actual
                = QueryBuilder
                .OrderByDescending<Person>(x => x.Age)
                .ThenByDescending(x => x.ModifiedAt)
                .Build(this.DbProvider);
            var expect =
@"order by
    [Age] desc,
    [ModifiedAt] desc";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }
    }
}
