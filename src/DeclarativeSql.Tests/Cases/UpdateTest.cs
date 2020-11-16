using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class UpdateTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void AllColumns()
        {
            var actual = QueryBuilder.Update<Person>(this.DbProvider);
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [Age] = @Age,
    [HasChildren] = @HasChildren,
    [ModifiedAt] = @ModifiedAt";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void OneColumn()
        {
            var actual = QueryBuilder.Update<Person>(this.DbProvider, x => x.Name);
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [ModifiedAt] = @ModifiedAt";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void OneColumn_AnonymousType()
        {
            var actual = QueryBuilder.Update<Person>(this.DbProvider, x => new { x.Name });
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [ModifiedAt] = @ModifiedAt";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void TwoColumns()
        {
            var actual = QueryBuilder.Update<Person>(this.DbProvider, x => new { x.Name, x.CreatedAt });
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [ModifiedAt] = @ModifiedAt";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void TwoColumns_WithCondition()
        {
            var actual = QueryBuilder.Update<Person>(this.DbProvider, x => x.Age >= 30, x => new { x.Name, x.CreatedAt });
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [ModifiedAt] = @ModifiedAt
where
    [Age] >= @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().NotBeNull();
            actual.BindParameter!.Count.Should().Be(1);
            actual.BindParameter.Should().Contain("p1", 30);
        }
    }
}
