using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class InsertTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void CreatedAt_PreferAttribute()
        {
            var actual = QueryBuilder.Insert<Person>().Build(this.DbProvider);
            var expect =
@"insert into [dbo].[Person]
(
    [名前],
    [Age],
    [HasChildren],
    [CreatedAt],
    [ModifiedAt]
)
values
(
    @Name,
    @Age,
    @HasChildren,
    SYSDATETIME(),
    @ModifiedAt
)";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void CreatedAt_PreferProperty()
        {
            var actual = QueryBuilder.Insert<Person>(ValuePriority.Property).Build(this.DbProvider);
            var expect =
@"insert into [dbo].[Person]
(
    [名前],
    [Age],
    [HasChildren],
    [CreatedAt],
    [ModifiedAt]
)
values
(
    @Name,
    @Age,
    @HasChildren,
    @CreatedAt,
    @ModifiedAt
)";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }
    }
}
