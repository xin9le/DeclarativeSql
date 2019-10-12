using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class MethodChainTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void Count_Where()
        {
            var actual = this.DbProvider.QueryBuilder.Count<Person>().Where(x => x.Id == 1).Build();
            var expect =
@"select count(*) as Count from [dbo].[Person]
where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void Select_Where()
        {
            var actual = this.DbProvider.QueryBuilder.Select<Person>().Where(x => x.Id == 1).Build();
            var expect =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren,
    [CreatedAt] as CreatedAt,
    [ModifiedAt] as ModifiedAt
from [dbo].[Person]
where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void Update_Where()
        {
            var actual = this.DbProvider.QueryBuilder.Update<Person>().Where(x => x.Id == 1).Build();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [Age] = @Age,
    [HasChildren] = @HasChildren,
    [ModifiedAt] = @ModifiedAt
where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void Delete_Where()
        {
            var actual = this.DbProvider.QueryBuilder.Delete<Person>().Where(x => x.Id == 1).Build();
            var expect =
@"delete from [dbo].[Person]
where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void Select_OrderBy()
        {
            var actual = this.DbProvider.QueryBuilder.Select<Person>().OrderBy(x => x.Id).Build();
            var expect =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren,
    [CreatedAt] as CreatedAt,
    [ModifiedAt] as ModifiedAt
from [dbo].[Person]
order by
    [Id]";
            actual.Statement.Should().Be(expect);
        }


        [Fact]
        public void Select_OrderByDescending()
        {
            var actual = this.DbProvider.QueryBuilder.Select<Person>().OrderByDescending(x => x.Id).Build();
            var expect =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren,
    [CreatedAt] as CreatedAt,
    [ModifiedAt] as ModifiedAt
from [dbo].[Person]
order by
    [Id] desc";
            actual.Statement.Should().Be(expect);
        }


        [Fact]
        public void Select_Where_OrderBy_ThenBy_ThenByDescending()
        {
            var actual
                = this.DbProvider.QueryBuilder
                .Select<Person>()
                .Where(x => x.Id == 1)
                .OrderBy(x => x.Id)
                .ThenBy(x => x.Name)
                .ThenByDescending(x => x.Age)
                .Build();
            var expect =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren,
    [CreatedAt] as CreatedAt,
    [ModifiedAt] as ModifiedAt
from [dbo].[Person]
where
    [Id] = @p1
order by
    [Id],
    [名前],
    [Age] desc";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }
    }
}
