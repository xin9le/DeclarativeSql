using DeclarativeSql.Sql;
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
            var actual = QueryBuilder.Select<Person>().Build(this.DbProvider);
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
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void OneColumn()
        {
            var actual = QueryBuilder.Select<Person>(x => x.Name).Build(this.DbProvider);
            var expect =
@"select
    [名前] as Name
from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void OneColumn_AnonymousType()
        {
            var actual = QueryBuilder.Select<Person>(x => new { x.Name }).Build(this.DbProvider);
            var expect =
@"select
    [名前] as Name
from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void TwoColumns()
        {
            var actual = QueryBuilder.Select<Person>(x => new { x.Name, x.Age }).Build(this.DbProvider);
            var expect =
@"select
    [名前] as Name,
    [Age] as Age
from [dbo].[Person]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }
    }
}
