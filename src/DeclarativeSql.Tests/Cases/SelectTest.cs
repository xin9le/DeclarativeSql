using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;

namespace DeclarativeSql.Tests.Cases;



public class SelectTest
{
    private DbProvider DbProvider { get; } = DbProvider.SqlServer;


    [Fact]
    public void AllColumns()
    {
        var actual = QueryBuilder.Select<Person>(this.DbProvider);
        var expect =
@"select
    [Id] as [Id],
    [名前] as [Name],
    [Age] as [Age],
    [HasChildren] as [HasChildren],
    [CreatedAt] as [CreatedAt],
    [ModifiedAt] as [ModifiedAt]
from [dbo].[Person]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }


    [Fact]
    public void OneColumn()
    {
        var actual = QueryBuilder.Select<Person>(this.DbProvider, x => x.Name);
        var expect =
@"select
    [名前] as [Name]
from [dbo].[Person]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }


    [Fact]
    public void OneColumn_AnonymousType()
    {
        var actual = QueryBuilder.Select<Person>(this.DbProvider, x => new { x.Name });
        var expect =
@"select
    [名前] as [Name]
from [dbo].[Person]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }


    [Fact]
    public void TwoColumns()
    {
        var actual = QueryBuilder.Select<Person>(this.DbProvider, x => new { x.Name, x.Age });
        var expect =
@"select
    [名前] as [Name],
    [Age] as [Age]
from [dbo].[Person]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }


    [Fact]
    public void OneColumn_Cast()
    {
        var actual = QueryBuilder.Select<Person>(this.DbProvider, x => (object)x.Name);
        var expect =
@"select
    [名前] as [Name]
from [dbo].[Person]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }


    [Fact]
    public void OneColumn_AnonymousType_Cast()
    {
        var actual = QueryBuilder.Select<Person>(this.DbProvider, x => (object)new { x.Name });
        var expect =
@"select
    [名前] as [Name]
from [dbo].[Person]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }


    [Fact]
    public void TwoColumns_Cast()
    {
        var actual = QueryBuilder.Select<Person>(this.DbProvider, x => (object)new { x.Name, x.Age });
        var expect =
@"select
    [名前] as [Name],
    [Age] as [Age]
from [dbo].[Person]";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().BeNull();
    }


    [Fact]
    public void WithCondition()
    {
        var actual = QueryBuilder.Select<Person>(this.DbProvider, x => x.Age >= 30, x => new { x.Name, x.Age });
        var expect =
@"select
    [名前] as [Name],
    [Age] as [Age]
from [dbo].[Person]
where
    [Age] >= @p1";
        actual.Statement.Should().Be(expect);
        actual.BindParameter.Should().NotBeNull();
        actual.BindParameter!.Count.Should().Be(1);
        actual.BindParameter.Should().Contain("p1", 30);
    }
}
