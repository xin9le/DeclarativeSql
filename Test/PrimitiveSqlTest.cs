﻿using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace DeclarativeSql.Tests
{
    [TestClass]
    public class PrimitiveSqlTest
    {
#pragma warning disable 0618
        #region Count
        [TestMethod]
        public void Count文生成()
        {
            var actual1 = PrimitiveSql.CreateCount(DbKind.Unknown, typeof(Person));
            var actual2 = PrimitiveSql.CreateCount<Person>(DbKind.Unknown);
            var expect = "select count(*) as Count from dbo.Person";
            actual1.Is(expect);
            actual2.Is(expect);
        }
        #endregion


        #region Select
        [TestMethod]
        public void 全列のSelect文生成()
        {
            var actual1 = PrimitiveSql.CreateSelect(typeof(Person));
            var actual2 = PrimitiveSql.CreateSelect<Person>();
            var expect =
@"select
    Id as Id,
    名前 as Name,
    Age as Age,
    HasChildren as HasChildren
from dbo.Person";
            actual1.Is(expect);
            actual2.Is(expect);
        }


        [TestMethod]
        public void 特定1列のSelect文生成()
        {
            var actual1 = PrimitiveSql.CreateSelect(typeof(Person), "Name");
            var actual2 = PrimitiveSql.CreateSelect<Person>(x => x.Name);
            var actual3 = PrimitiveSql.CreateSelect<Person>(x => new { x.Name });
            var expect =
@"select
    名前 as Name
from dbo.Person";
            actual1.Is(expect);
            actual2.Is(expect);
            actual3.Is(expect);
        }


        [TestMethod]
        public void 特定2列のSelect文生成()
        {
            var actual1 = PrimitiveSql.CreateSelect(typeof(Person), "Name", "Age");
            var actual2 = PrimitiveSql.CreateSelect<Person>(x => x.Name, x => x.Age);
            var actual3 = PrimitiveSql.CreateSelect<Person>(x => new { x.Name, x.Age });
            var expect =
@"select
    名前 as Name,
    Age as Age
from dbo.Person";
            actual1.Is(expect);
            actual2.Is(expect);
            actual3.Is(expect);
        }
        #endregion


        #region Insert
        [TestMethod]
        public void シーケンスを利用するInsert文生成()
        {
            var actual1 = PrimitiveSql.CreateInsert(DbKind.SqlServer, typeof(Person));
            var actual2 = PrimitiveSql.CreateInsert<Person>(DbKind.SqlServer);
            var expect =
@"insert into dbo.[Person]
(
    名前,
    Age,
    HasChildren
)
values
(
    @Name,
    next value for dbo.AgeSeq,
    @HasChildren
)";
            actual1.Is(expect);
            actual2.Is(expect);
        }


        [TestMethod]
        public void シーケンスを利用しないInsert文生成()
        {
            var actual1 = PrimitiveSql.CreateInsert(DbKind.SqlServer, typeof(Person), false);
            var actual2 = PrimitiveSql.CreateInsert<Person>(DbKind.SqlServer, false);
            var expect =
@"insert into dbo.[Person]
(
    名前,
    Age,
    HasChildren
)
values
(
    @Name,
    @Age,
    @HasChildren
)";
            actual1.Is(expect);
            actual2.Is(expect);
        }


        [TestMethod]
        public void IDを設定するInsert文生成()
        {
            var actual1 = PrimitiveSql.CreateInsert(DbKind.SqlServer, typeof(Person), setIdentity: true);
            var actual2 = PrimitiveSql.CreateInsert<Person>(DbKind.SqlServer, setIdentity: true);
            var expect =
@"insert into dbo.[Person]
(
    Id,
    名前,
    Age,
    HasChildren
)
values
(
    @Id,
    @Name,
    next value for dbo.AgeSeq,
    @HasChildren
)";
            actual1.Is(expect);
            actual2.Is(expect);
        }
        #endregion


        #region Update
        [TestMethod]
        public void 全列のUpdate文生成()
        {
            var actual1 = PrimitiveSql.CreateUpdate(DbKind.SqlServer, typeof(Person));
            var actual2 = PrimitiveSql.CreateUpdate<Person>(DbKind.SqlServer);
            var expect =
@"update dbo.[Person]
set
    名前 = @Name,
    Age = @Age,
    HasChildren = @HasChildren";
            actual1.Is(expect);
            actual2.Is(expect);
        }


        [TestMethod]
        public void 特定1列のUpdate文生成()
        {
            var actual1 = PrimitiveSql.CreateUpdate(DbKind.SqlServer, typeof(Person), new [] { "Name" });
            var actual2 = PrimitiveSql.CreateUpdate<Person>(DbKind.SqlServer, x => x.Name);
            var actual3 = PrimitiveSql.CreateUpdate<Person>(DbKind.SqlServer, x => new { x.Name });
            var expect =
@"update dbo.[Person]
set
    名前 = @Name";
            actual1.Is(expect);
            actual2.Is(expect);
            actual3.Is(expect);
        }


        [TestMethod]
        public void 特定2列のUpdate文生成()
        {
            var actual1 = PrimitiveSql.CreateUpdate(DbKind.SqlServer, typeof(Person), new [] { "Name", "Age" });
            var actual2 = PrimitiveSql.CreateUpdate<Person>(DbKind.SqlServer, x => x.Name, x => x.Age);
            var actual3 = PrimitiveSql.CreateUpdate<Person>(DbKind.SqlServer, x => new { x.Name, x.Age });
            var expect =
@"update dbo.[Person]
set
    名前 = @Name,
    Age = @Age";
            actual1.Is(expect);
            actual2.Is(expect);
            actual3.Is(expect);
        }


        [TestMethod]
        public void IDを設定するUpdate文生成()
        {
            var actual1 = PrimitiveSql.CreateUpdate(DbKind.SqlServer, typeof(Person), setIdentity: true);
            var actual2 = PrimitiveSql.CreateUpdate<Person>(DbKind.SqlServer, setIdentity: true);
            var expect =
@"update dbo.[Person]
set
    Id = @Id,
    名前 = @Name,
    Age = @Age,
    HasChildren = @HasChildren";
            actual1.Is(expect);
            actual2.Is(expect);
        }
        #endregion


        #region Delete
        [TestMethod]
        public void Delete文生成()
        {
            var actual1 = PrimitiveSql.CreateDelete(DbKind.Unknown, typeof(Person));
            var actual2 = PrimitiveSql.CreateDelete<Person>(DbKind.Unknown);
            var expect = "delete from dbo.Person";
            actual1.Is(expect);
            actual2.Is(expect);
        }
        #endregion


        #region Truncate
        [TestMethod]
        public void Truncate文生成()
        {
            var actual1 = PrimitiveSql.CreateTruncate(DbKind.Unknown, typeof(Person));
            var actual2 = PrimitiveSql.CreateTruncate<Person>(DbKind.Unknown);
            var expect = "truncate table dbo.Person";
            actual1.Is(expect);
            actual2.Is(expect);
        }
        #endregion
#pragma warning restore 0618
    }
}