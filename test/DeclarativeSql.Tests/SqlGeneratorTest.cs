using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using DeclarativeSql.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace DeclarativeSql.Tests
{
    [TestClass]
    public class SqlGeneratorTest
    {
        //--- とりあえず決め打ちで確認
        protected DbProvider DbProvider { get; } = DbProvider.SqlServer;


        #region Count
        [TestMethod]
        public void Count文生成()
        {
            var actual = this.DbProvider.Count<Person>().ToString();
            var expect = "select count(*) as Count from [dbo].[Person]";
            actual.Is(expect);
        }
        #endregion


        #region Select
        [TestMethod]
        public void 全列のSelect文生成()
        {
            var actual = this.DbProvider.Select<Person>().ToString();
            var expect =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren
from [dbo].[Person]";
            actual.Is(expect);
        }


        [TestMethod]
        public void 特定1列のSelect文生成()
        {
            var actual = this.DbProvider.Select<Person>(x => new { x.Name }).ToString();
            var expect =
@"select
    [名前] as Name
from [dbo].[Person]";
            actual.Is(expect);
        }


        [TestMethod]
        public void 特定2列のSelect文生成()
        {
            var actual = this.DbProvider.Select<Person>(x => new { x.Name, x.Age }).ToString();
            var expect =
@"select
    [名前] as Name,
    [Age] as Age
from [dbo].[Person]";
            actual.Is(expect);
        }
        #endregion


        #region Insert
        [TestMethod]
        public void シーケンスを利用するInsert文生成()
        {
            var actual = this.DbProvider.Insert<Person>().ToString();
            var expect =
@"insert into [dbo].[Person]
(
    [名前],
    [Age],
    [HasChildren]
)
values
(
    @Name,
    next value for [dbo].[AgeSeq],
    @HasChildren
)";
            actual.Is(expect);
        }


        [TestMethod]
        public void シーケンスを利用しないInsert文生成()
        {
            var actual = this.DbProvider.Insert<Person>(false).ToString();
            var expect =
@"insert into [dbo].[Person]
(
    [名前],
    [Age],
    [HasChildren]
)
values
(
    @Name,
    @Age,
    @HasChildren
)";
            actual.Is(expect);
        }


        [TestMethod]
        public void IDを設定するInsert文生成()
        {
            var actual = this.DbProvider.Insert<Person>(setIdentity: true).ToString();
            var expect =
@"insert into [dbo].[Person]
(
    [Id],
    [名前],
    [Age],
    [HasChildren]
)
values
(
    @Id,
    @Name,
    next value for [dbo].[AgeSeq],
    @HasChildren
)";
            actual.Is(expect);
        }
        #endregion


        #region Update
        [TestMethod]
        public void 全列のUpdate文生成()
        {
            var actual = this.DbProvider.Update<Person>().ToString();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [Age] = @Age,
    [HasChildren] = @HasChildren";
            actual.Is(expect);
        }


        [TestMethod]
        public void 特定1列のUpdate文生成()
        {
            var actual1 = this.DbProvider.Update<Person>(x => x.Name).ToString();
            var actual2 = this.DbProvider.Update<Person>(x => new { x.Name }).ToString();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name";
            actual1.Is(expect);
            actual2.Is(expect);
        }


        [TestMethod]
        public void 特定2列のUpdate文生成()
        {
            var actual = this.DbProvider.Update<Person>(x => new { x.Name, x.Age }).ToString();
            var expect =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [Age] = @Age";
            actual.Is(expect);
        }


        [TestMethod]
        public void IDを設定するUpdate文生成()
        {
            var actual = this.DbProvider.Update<Person>(setIdentity: true).ToString();
            var expect =
@"update [dbo].[Person]
set
    [Id] = @Id,
    [名前] = @Name,
    [Age] = @Age,
    [HasChildren] = @HasChildren";
            actual.Is(expect);
        }
        #endregion


        #region Delete
        [TestMethod]
        public void Delete文生成()
        {
            var actual = this.DbProvider.Delete<Person>().ToString();
            var expect = "delete from [dbo].[Person]";
            actual.Is(expect);
        }
        #endregion


        #region Truncate
        [TestMethod]
        public void Truncate文生成()
        {
            var actual = this.DbProvider.Truncate<Person>().ToString();
            var expect = "truncate table [dbo].[Person]";
            actual.Is(expect);
        }
        #endregion


        #region Where
        [TestMethod]
        public void 等しい()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id == 1).Build();

            var expectStatement =
@"where
    [Id] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 等しくない()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id != 1).Build();

            var expectStatement =
@"where
    [Id] <> @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void より大きい()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id > 1).Build();

            var expectStatement =
@"where
    [Id] > @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void より小さい()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id < 1).Build();

            var expectStatement =
@"where
    [Id] < @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 以上()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id >= 1).Build();

            var expectStatement =
@"where
    [Id] >= @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 以下()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id <= 1).Build();

            var expectStatement =
@"where
    [Id] <= @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Null()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Name == null).Build();

            var expectStatement =
@"where
    [名前] is null";
            IDictionary<string, object> expectParameter = new ExpandoObject();

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 非Null()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Name != null).Build();

            var expectStatement =
@"where
    [名前] is not null";
            IDictionary<string, object> expectParameter = new ExpandoObject();

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void And()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id > 1 && x.Name == "xin9le").Build();

            var expectStatement =
@"where
    [Id] > @p0 and [名前] = @p1";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Or()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id > 1 || x.Name == "xin9le").Build();

            var expectStatement =
@"where
    [Id] > @p0 or [名前] = @p1";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void AndOr1()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id > 1 && x.Name == "xin9le" || x.Age <= 30).Build();

            var expectStatement =
@"where
    ([Id] > @p0 and [名前] = @p1) or [Age] <= @p2";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");
            expectParameter.Add("p2", 30);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void AndOr2()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id > 1 && (x.Name == "xin9le" || x.Age <= 30)).Build();

            var expectStatement =
@"where
    [Id] > @p0 and ([名前] = @p1 or [Age] <= @p2)";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");
            expectParameter.Add("p2", 30);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void AndOr3()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Id > 1 || x.Name == "xin9le" && x.Age <= 30).Build();

            var expectStatement =
@"where
    [Id] > @p0 or ([名前] = @p1 and [Age] <= @p2)";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");
            expectParameter.Add("p2", 30);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void AndOr4()
        {
            var actual = this.DbProvider.Where<Person>(x => (x.Id > 1 || x.Name == "xin9le") && x.Age <= 30).Build();

            var expectStatement =
@"where
    ([Id] > @p0 or [名前] = @p1) and [Age] <= @p2";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");
            expectParameter.Add("p2", 30);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Contains()
        {
            var value = Enumerable.Range(0, 3).Cast<object>().ToArray();
            var actual = this.DbProvider.Where<Person>(x => value.Contains(x.Id)).Build();

            var expectStatement =
@"where
    [Id] in @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", value);

            actual.WhereParameters.IsStructuralEqual(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Contains1000件超()
        {
            var value1 = Enumerable.Range(0, 1000).Cast<object>().ToArray();
            var value2 = Enumerable.Range(1000, 234).Cast<object>().ToArray();
            var value  = value1.Concat(value2);
            var actual = this.DbProvider.Where<Person>(x => value.Contains(x.Id)).Build();

            var expectStatement =
@"where
    [Id] in @p0 or [Id] in @p1";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", value2);
            expectParameter.Add("p1", value1);

            actual.WhereParameters.IsStructuralEqual(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 右辺が変数()
        {
            var id = 1;
            var actual = this.DbProvider.Where<Person>(x => x.Id == id).Build();

            var expectStatement =
@"where
    [Id] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", id);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 右辺がコンストラクタ()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Name == new string('a', 3)).Build();

            var expectStatement =
@"where
    [名前] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", "aaa");

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        //[TestMethod]
        //public void 右辺が配列()
        //{}


        [TestMethod]
        public void 右辺がメソッド()
        {
            var some = new AccessorProvider();
            var actual = this.DbProvider.Where<Person>(x => x.Name == some.InstanceMethod()).Build();

            var expectStatement =
@"where
    [名前] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", some.InstanceMethod());

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 右辺がラムダ式()
        {
            Func<int, string> getName = x => x.ToString();
            var actual = this.DbProvider.Where<Person>(x => x.Name == getName(123)).Build();

            var expectStatement =
@"where
    [名前] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", "123");

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 右辺がプロパティ()
        {
            var some = new AccessorProvider();
            var actual = this.DbProvider.Where<Person>(x => x.Age == some.InstanceProperty).Build();

            var expectStatement =
@"where
    [Age] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", some.InstanceProperty);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 右辺がインデクサ()
        {
            var ids = new [] { 1, 2, 3 };
            var actual = this.DbProvider.Where<Person>(x => x.Id == ids[0]).Build();

            var expectStatement =
@"where
    [Id] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", ids[0]);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 右辺が静的メソッド()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Name == AccessorProvider.StaticMethod()).Build();

            var expectStatement =
@"where
    [名前] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", AccessorProvider.StaticMethod());

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 右辺が静的プロパティ()
        {
            var actual = this.DbProvider.Where<Person>(x => x.Age == AccessorProvider.StaticProperty).Build();

            var expectStatement =
@"where
    [Age] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", AccessorProvider.StaticProperty);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Boolean()
        {
            var actual = this.DbProvider.Where<Person>(x => x.HasChildren).Build();

            var expectStatement =
@"where
    [HasChildren] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", true);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void InverseBoolean()
        {
            var actual = this.DbProvider.Where<Person>(x => !x.HasChildren).Build();

            var expectStatement =
@"where
    [HasChildren] <> @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", true);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void BooleanAndOr()
        {
            var actual = this.DbProvider.Where<Person>(x => x.HasChildren == true || x.Id != 0 || x.Name == "xin9le" && !x.HasChildren).Build();

            var expectStatement =
@"where
    [HasChildren] = @p0 or [Id] <> @p1 or ([名前] = @p2 and [HasChildren] <> @p3)";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", true);
            expectParameter.Add("p1", 0);
            expectParameter.Add("p2", "xin9le");
            expectParameter.Add("p3", true);

            actual.WhereParameters.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }
        #endregion


        #region OrderBy
        [TestMethod]
        public void OrderBy文生成()
        {
            var actual = this.DbProvider.OrderBy<Person>(x => x.Name).ToString();
            var expect =
@"order by
    [名前]";
            actual.Is(expect);
        }


        [TestMethod]
        public void OrderByDescending文生成()
        {
            var actual = this.DbProvider.OrderByDescending<Person>(x => x.Age).ToString();
            var expect =
@"order by
    [Age] desc";
            actual.Is(expect);
        }
        #endregion


        #region Clause Chain
        [TestMethod]
        public void Count_Where()
        {
            var actual = this.DbProvider.Count<Person>().Where(x => x.Id == 1).Build();

            var expectStatement =
@"select count(*) as Count from [dbo].[Person]
where
    [Id] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Statement.Is(expectStatement);
            actual.WhereParameters.Is(expectParameter);
        }


        [TestMethod]
        public void Select_Where()
        {
            var actual = this.DbProvider.Select<Person>().Where(x => x.Id == 1).Build();

            var expectStatement =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren
from [dbo].[Person]
where
    [Id] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Statement.Is(expectStatement);
            actual.WhereParameters.Is(expectParameter);
        }


        [TestMethod]
        public void Update_Where()
        {
            var actual = this.DbProvider.Update<Person>().Where(x => x.Id == 1).Build();

            var expectStatement =
@"update [dbo].[Person]
set
    [名前] = @Name,
    [Age] = @Age,
    [HasChildren] = @HasChildren
where
    [Id] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Statement.Is(expectStatement);
            actual.WhereParameters.Is(expectParameter);
        }


        [TestMethod]
        public void Delete_Where()
        {
            var actual = this.DbProvider.Delete<Person>().Where(x => x.Id == 1).Build();

            var expectStatement =
@"delete from [dbo].[Person]
where
    [Id] = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Statement.Is(expectStatement);
            actual.WhereParameters.Is(expectParameter);
        }


        [TestMethod]
        public void Select_OrderBy()
        {
            var actual = this.DbProvider.Select<Person>().OrderBy(x => x.Id).ToString();
            var expect =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren
from [dbo].[Person]
order by
    [Id]";
            actual.Is(expect);
        }

        
        [TestMethod]
        public void Select_OrderByDescending()
        {
            var actual = this.DbProvider.Select<Person>().OrderByDescending(x => x.Id).ToString();
            var expect =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren
from [dbo].[Person]
order by
    [Id] desc";
            actual.Is(expect);
        }

        
        [TestMethod]
        public void Select_Where_OrderBy_ThenBy_ThenByDescending()
        {
            var actual = this.DbProvider
                        .Select<Person>()
                        .Where(x => x.Id == 1)
                        .OrderBy(x => x.Id)
                        .ThenBy(x => x.Name)
                        .ThenByDescending(x => x.Age)
                        .Build();

            var expectStatement =
@"select
    [Id] as Id,
    [名前] as Name,
    [Age] as Age,
    [HasChildren] as HasChildren
from [dbo].[Person]
where
    [Id] = @p0
order by
    [Id],
    [名前],
    [Age] desc";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Statement.Is(expectStatement);
            actual.WhereParameters.Is(expectParameter);
        }
        #endregion
    }
}