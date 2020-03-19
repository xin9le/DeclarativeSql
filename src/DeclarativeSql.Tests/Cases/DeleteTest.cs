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
        public void Create()
        {
            var actual = QueryBuilder.Delete<Person>(this.DbProvider);
            var expect = "delete from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }
    }
}
