using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class DeleteTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void Create()
        {
            var actual = this.DbProvider.QueryBuilder.Delete<Person>().ToString();
            var expect = "delete from [dbo].[Person]";
            actual.Should().Be(expect);
        }
    }
}
