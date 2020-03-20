using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class TruncateTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void Create()
        {
            var actual = QueryBuilder.Truncate<Person>(this.DbProvider);
            var expect = "truncate table [dbo].[Person]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }
    }
}
