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
            var actual = this.DbProvider.QueryBuilder.Truncate<Person>().ToString();
            var expect = "truncate table [dbo].[Person]";
            actual.Should().Be(expect);
        }
    }
}
