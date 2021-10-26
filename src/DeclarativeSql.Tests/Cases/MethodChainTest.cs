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
            var actual = CreateActualQuery();
            var expect =
@"select count(*) as [Count] from [dbo].[Person]
where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().NotBeNull();
            actual.BindParameter.Should().Contain("p1", 1);

            Query CreateActualQuery()
            {
                using (var builder = new QueryBuilder<Person>(this.DbProvider))
                {
                    builder.Count();
                    builder.Where(x => x.Id == 1);
                    return builder.Build();
                }
            }
        }


        [Fact]
        public void Select_Where()
        {
            var actual = CreateActualQuery();
            var expect =
@"select
    [Id] as [Id],
    [名前] as [Name],
    [Age] as [Age],
    [HasChildren] as [HasChildren],
    [CreatedAt] as [CreatedAt],
    [ModifiedAt] as [ModifiedAt]
from [dbo].[Person]
where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().NotBeNull();
            actual.BindParameter.Should().Contain("p1", 1);

            Query CreateActualQuery()
            {
                using (var builder = new QueryBuilder<Person>(this.DbProvider))
                {
                    builder.Select();
                    builder.Where(x => x.Id == 1);
                    return builder.Build();
                }
            }
        }


        [Fact]
        public void Update_Where()
        {
            var actual = CreateActualQuery();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [Age] = @Age,
    [HasChildren] = @HasChildren,
    [ModifiedAt] = @ModifiedAt
where
    [Id] = @p5";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().NotBeNull();
            actual.BindParameter.Should().Contain("p5", 1);

            Query CreateActualQuery()
            {
                using (var builder = new QueryBuilder<Person>(this.DbProvider))
                {
                    builder.Update();
                    builder.Where(x => x.Id == 1);
                    return builder.Build();
                }
            }
        }


        [Fact]
        public void Delete_Where()
        {
            var actual = CreateActualQuery();
            var expect =
@"delete from [dbo].[Person]
where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().NotBeNull();
            actual.BindParameter.Should().Contain("p1", 1);

            Query CreateActualQuery()
            {
                using (var builder = new QueryBuilder<Person>(this.DbProvider))
                {
                    builder.Delete();
                    builder.Where(x => x.Id == 1);
                    return builder.Build();
                }
            }
        }


        [Fact]
        public void Select_OrderBy()
        {
            var actual = CreateActualQuery();
            var expect =
@"select
    [Id] as [Id],
    [名前] as [Name],
    [Age] as [Age],
    [HasChildren] as [HasChildren],
    [CreatedAt] as [CreatedAt],
    [ModifiedAt] as [ModifiedAt]
from [dbo].[Person]
order by
    [Id]";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();

            Query CreateActualQuery()
            {
                using (var builder = new QueryBuilder<Person>(this.DbProvider))
                {
                    builder.Select();
                    builder.OrderBy(x => x.Id);
                    return builder.Build();
                }
            }
        }


        [Fact]
        public void Select_OrderByDescending()
        {
            var actual = CreateActualQuery();
            var expect =
@"select
    [Id] as [Id],
    [名前] as [Name],
    [Age] as [Age],
    [HasChildren] as [HasChildren],
    [CreatedAt] as [CreatedAt],
    [ModifiedAt] as [ModifiedAt]
from [dbo].[Person]
order by
    [Id] desc";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();

            Query CreateActualQuery()
            {
                using (var builder = new QueryBuilder<Person>(this.DbProvider))
                {
                    builder.Select();
                    builder.OrderByDescending(x => x.Id);
                    return builder.Build();
                }
            }
        }


        [Fact]
        public void Select_Where_OrderBy_ThenBy_ThenByDescending()
        {
            var actual = CreateActualQuery();
            var expect =
@"select
    [Id] as [Id],
    [名前] as [Name],
    [Age] as [Age],
    [HasChildren] as [HasChildren],
    [CreatedAt] as [CreatedAt],
    [ModifiedAt] as [ModifiedAt]
from [dbo].[Person]
where
    [Id] = @p1
order by
    [Id],
    [名前],
    [Age] desc";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().NotBeNull();
            actual.BindParameter.Should().Contain("p1", 1);

            Query CreateActualQuery()
            {
                using (var builder = new QueryBuilder<Person>(this.DbProvider))
                {
                    builder.Select();
                    builder.Where(x => x.Id == 1);
                    builder.OrderBy(x => x.Id);
                    builder.ThenBy(x => x.Name);
                    builder.ThenByDescending(x => x.Age);
                    return builder.Build();
                }
            }
        }
    }
}
