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
                = this.DbProvider.QueryBuilder
                .OrderBy<Person>(x => x.Name)
                .ThenBy(x => x.CreatedAt)
                .Build();
            var expect =
@"order by
    [名前],
    [CreatedAt]";
            actual.Statement.Should().Be(expect);
        }


        [Fact]
        public void Descending()
        {
            var actual
                = this.DbProvider.QueryBuilder
                .OrderByDescending<Person>(x => x.Age)
                .ThenByDescending(x => x.ModifiedAt)
                .Build();
            var expect =
@"order by
    [Age] desc,
    [ModifiedAt] desc";
            actual.Statement.Should().Be(expect);
        }
    }
}
