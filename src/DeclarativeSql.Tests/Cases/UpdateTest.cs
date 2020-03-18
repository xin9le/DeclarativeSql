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
            var actual = QueryBuilder.Update<Person>().Build(this.DbProvider);
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
            var actual = QueryBuilder.Update<Person>(x => x.Name).Build(this.DbProvider);
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
            var actual = QueryBuilder.Update<Person>(x => new { x.Name }).Build(this.DbProvider);
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
            var actual = QueryBuilder.Update<Person>(x => new { x.Name, x.CreatedAt }).Build(this.DbProvider);
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [ModifiedAt] = @ModifiedAt";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }
    }
}
