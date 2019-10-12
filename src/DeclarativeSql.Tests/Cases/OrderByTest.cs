using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class OrderByTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void Create()
        {
            var actual = this.DbProvider.QueryBuilder.Count<Person>().ToString();
            var expect = "select count(*) as Count from [dbo].[Person]";
            actual.Should().Be(expect);
        }
    }
}
