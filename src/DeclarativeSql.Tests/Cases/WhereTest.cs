using System;
using System.Linq;
using DeclarativeSql.Sql;
using DeclarativeSql.Tests.Models;
using FluentAssertions;
using Xunit;



namespace DeclarativeSql.Tests.Cases
{
    public class WhereTest
    {
        private DbProvider DbProvider { get; } = DbProvider.SqlServer;


        [Fact]
        public void Equal()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id == 1).Build(this.DbProvider);
            var expect =
@"where
    [Id] = @p1";

            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void NotEqual()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id != 1).Build(this.DbProvider);
            var expect =
@"where
    [Id] <> @p1";

            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void GreaterThan()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id > 1).Build(this.DbProvider);
            var expect =
@"where
    [Id] > @p1";

            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void LessThan()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id < 1).Build(this.DbProvider);
            var expect =
@"where
    [Id] < @p1";

            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void GreaterThanOrEqual()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id >= 1).Build(this.DbProvider);
            var expect =
@"where
    [Id] >= @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void LessThanOrEqual()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id <= 1).Build(this.DbProvider);
            var expect =
@"where
    [Id] <= @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
        }


        [Fact]
        public void Null()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Name == null).Build(this.DbProvider);
            var expect =
@"where
    [名前] is null";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void NotNull()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Name != null).Build(this.DbProvider);
            var expect =
@"where
    [名前] is not null";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().BeNull();
        }


        [Fact]
        public void And()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id > 1 && x.Name == "xin9le").Build(this.DbProvider);
            var expect =
@"where
    [Id] > @p1 and [名前] = @p2";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
            actual.BindParameter.Should().Contain("p2", "xin9le");
        }


        [Fact]
        public void Or()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id > 1 || x.Name == "xin9le").Build(this.DbProvider);
            var expect =
@"where
    [Id] > @p1 or [名前] = @p2";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
            actual.BindParameter.Should().Contain("p2", "xin9le");
        }


        [Fact]
        public void AndOr1()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id > 1 && x.Name == "xin9le" || x.Age <= 30).Build(this.DbProvider);
            var expect =
@"where
    ([Id] > @p1 and [名前] = @p2) or [Age] <= @p3";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
            actual.BindParameter.Should().Contain("p2", "xin9le");
            actual.BindParameter.Should().Contain("p3", 30);
        }


        [Fact]
        public void AndOr2()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id > 1 && (x.Name == "xin9le" || x.Age <= 30)).Build(this.DbProvider);
            var expect =
@"where
    [Id] > @p1 and ([名前] = @p2 or [Age] <= @p3)";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
            actual.BindParameter.Should().Contain("p2", "xin9le");
            actual.BindParameter.Should().Contain("p3", 30);
        }


        [Fact]
        public void AndOr3()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Id > 1 || x.Name == "xin9le" && x.Age <= 30).Build(this.DbProvider);
            var expect =
@"where
    [Id] > @p1 or ([名前] = @p2 and [Age] <= @p3)";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
            actual.BindParameter.Should().Contain("p2", "xin9le");
            actual.BindParameter.Should().Contain("p3", 30);
        }


        [Fact]
        public void AndOr4()
        {
            var actual = QueryBuilder.Where<Person>(x => (x.Id > 1 || x.Name == "xin9le") && x.Age <= 30).Build(this.DbProvider);
            var expect =
@"where
    ([Id] > @p1 or [名前] = @p2) and [Age] <= @p3";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", 1);
            actual.BindParameter.Should().Contain("p2", "xin9le");
            actual.BindParameter.Should().Contain("p3", 30);
        }


        [Fact]
        public void Contains()
        {
            var value = Enumerable.Range(0, 3).ToArray();
            var actual = QueryBuilder.Where<Person>(x => value.Contains(x.Id)).Build(this.DbProvider);
            var expect =
@"where
    [Id] in @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().ContainKey("p1");
            actual.BindParameter["p1"].Should().BeEquivalentTo(value);
        }


        [Fact]
        public void Contains_Over1000()
        {
            var value1 = Enumerable.Range(0, 1000).ToArray();
            var value2 = Enumerable.Range(1000, 234).ToArray();
            var value = value1.Concat(value2);
            var actual = QueryBuilder.Where<Person>(x => value.Contains(x.Id)).Build(this.DbProvider);
            var expect =
@"where
    [Id] in @p1 or [Id] in @p2";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().ContainKey("p1");
            actual.BindParameter["p1"].Should().BeEquivalentTo(value1);
            actual.BindParameter.Should().ContainKey("p2");
            actual.BindParameter["p2"].Should().BeEquivalentTo(value2);
        }


        [Fact]
        public void Variable()
        {
            var id = 1;
            var actual = QueryBuilder.Where<Person>(x => x.Id == id).Build(this.DbProvider);
            var expect =
@"where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", id);
        }


        [Fact]
        public void Constructor()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Name == new string('a', 3)).Build(this.DbProvider);
            var expect =
@"where
    [名前] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", "aaa");
        }


        [Fact]
        public void Array()
        {
            // do nothing
        }


        [Fact]
        public void InstanceMethod()
        {
            var some = new AccessorProvider();
            var actual = QueryBuilder.Where<Person>(x => x.Name == some.InstanceMethod()).Build(this.DbProvider);
            var expect =
@"where
    [名前] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", some.InstanceMethod());
        }


        [Fact]
        public void Lambda()
        {
            Func<int, string> getName = x => x.ToString();
            var actual = QueryBuilder.Where<Person>(x => x.Name == getName(123)).Build(this.DbProvider);
            var expect =
@"where
    [名前] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", "123");
        }


        [Fact]
        public void InstanceProperty()
        {
            var some = new AccessorProvider();
            var actual = QueryBuilder.Where<Person>(x => x.Age == some.InstanceProperty).Build(this.DbProvider);
            var expect =
@"where
    [Age] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", some.InstanceProperty);
        }


        [Fact]
        public void Indexer()
        {
            var ids = new[] { 1, 2, 3 };
            var actual = QueryBuilder.Where<Person>(x => x.Id == ids[0]).Build(this.DbProvider);
            var expect =
@"where
    [Id] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", ids[0]);
        }


        [Fact]
        public void StaticMethod()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Name == AccessorProvider.StaticMethod()).Build(this.DbProvider);
            var expect =
@"where
    [名前] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", AccessorProvider.StaticMethod());
        }


        [Fact]
        public void StaticProperty()
        {
            var actual = QueryBuilder.Where<Person>(x => x.Age == AccessorProvider.StaticProperty).Build(this.DbProvider);
            var expect =
@"where
    [Age] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", AccessorProvider.StaticProperty);
        }

/*
        [Fact]
        public void Boolean()
        {
            var actual = QueryBuilder.Where<Person>(x => x.HasChildren).Build(this.DbProvider);
            var expect =
@"where
    [HasChildren] = @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", true);
        }


        [Fact]
        public void InverseBoolean()
        {
            var actual = QueryBuilder.Where<Person>(x => !x.HasChildren).Build(this.DbProvider);
            var expect =
@"where
    [HasChildren] <> @p1";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", true);
        }


        [Fact]
        public void BooleanAndOr()
        {
            var actual = QueryBuilder.Where<Person>(x => x.HasChildren == true || x.Id != 0 || x.Name == "xin9le" && !x.HasChildren).Build(this.DbProvider);
            var expect =
@"where
    [HasChildren] = @p1 or [Id] <> @p2 or ([名前] = @p3 and [HasChildren] <> @p4)";
            actual.Statement.Should().Be(expect);
            actual.BindParameter.Should().Contain("p1", true);
            actual.BindParameter.Should().Contain("p2", 0);
            actual.BindParameter.Should().Contain("p3", "xin9le");
            actual.BindParameter.Should().Contain("p4", true);
        }
*/
    }
}
