using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class DeleteTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void All()
        {
            var actual = QueryBuilder.Delete<Person>(this.DbProvider);
            var expect = "delete from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void WithCondition()
        {
            var actual = QueryBuilder.Delete<Person>(this.DbProvider, x => x.Age >= 30);
            var expect =
@"delete from [dbo].[Person]
where
    [Age] >= @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Count.Should().Be(1);
            actual.BindParameter.Should().Contain("p1", 30);
        }
    }
}
