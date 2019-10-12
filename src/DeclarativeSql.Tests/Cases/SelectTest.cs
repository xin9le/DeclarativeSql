using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class SelectTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void AllColumns()
        {
            var actual = this.DbProvider.QueryBuilder.Select<Person>().Build();
            var expect =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren,
    [CreatedAt] as CreatedAt,
    [ModifiedAt] as ModifiedAt
from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
        }


        [Fact]
        public void OneColumn()
        {
            var actual = this.DbProvider.QueryBuilder.Select<Person>(x => x.Name).Build();
            var expect =
@"select
    [名前] as Name
from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
        }


        [Fact]
        public void OneColumn_AnonymousType()
        {
            var actual = this.DbProvider.QueryBuilder.Select<Person>(x => new { x.Name }).Build();
            var expect =
@"select
    [名前] as Name
from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
        }


        [Fact]
        public void TwoColumns()
        {
            var actual = this.DbProvider.QueryBuilder.Select<Person>(x => new { x.Name, x.Age }).Build();
            var expect =
@"select
    [名前] as Name,
    [Age] as Age
from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
        }
    }
}
