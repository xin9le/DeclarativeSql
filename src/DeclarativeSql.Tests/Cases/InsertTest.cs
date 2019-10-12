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
            var actual = this.DbProvider.QueryBuilder.Insert<Person>().ToString();
            var expect =
@"insert into [dbo].[Person]
(
    [名前],
    [Age],
    [CreatedAt],
    [ModifiedAt]
)
values
(
    @Name,
    @Age,
    SYSDATETIME(),
    @ModifiedAt
)";
            actual.Should().Be(expect);
        }


        [Fact]
        public void CreatedAt_PreferProperty()
        {
            var actual = this.DbProvider.QueryBuilder.Insert<Person>(ValuePriority.Property).ToString();
            var expect =
@"insert into [dbo].[Person]
(
    [名前],
    [Age],
    [CreatedAt],
    [ModifiedAt]
)
values
(
    @Name,
    @Age,
    @CreatedAt,
    @ModifiedAt
)";
            actual.Should().Be(expect);
        }
    }
}
