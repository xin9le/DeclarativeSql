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
            var actual = this.DbProvider.QueryBuilder.Update<Person>().ToString();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [Age] = @Age,
    [HasChildren] = @HasChildren,
    [ModifiedAt] = @ModifiedAt";
            actual.Should().Be(expect);
        }


        [Fact]
        public void OneColumn()
        {
            var actual = this.DbProvider.QueryBuilder.Update<Person>(x => x.Name).ToString();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [ModifiedAt] = @ModifiedAt";
            actual.Should().Be(expect);
        }


        [Fact]
        public void OneColumn_AnonymousType()
        {
            var actual = this.DbProvider.QueryBuilder.Update<Person>(x => new { x.Name }).ToString();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [ModifiedAt] = @ModifiedAt";
            actual.Should().Be(expect);
        }


        [Fact]
        public void TwoColumns()
        {
            var actual = this.DbProvider.QueryBuilder.Update<Person>(x => new { x.Name, x.CreatedAt }).ToString();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [ModifiedAt] = @ModifiedAt";
            actual.Should().Be(expect);
        }
    }
}
