using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace DeclarativeSql.Tests
{
    [TestClass]
    public class PredicateSqlTest
    {
        [TestMethod]
        public void 等しい()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id == 1);

            var expectStatement = "Id = @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 等しくない()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id != 1);

            var expectStatement = "Id <> @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void より大きい()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id > 1);

            var expectStatement = "Id > @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void より小さい()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id < 1);

            var expectStatement = "Id < @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 以上()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id >= 1);

            var expectStatement = "Id >= @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 以下()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id <= 1);

            var expectStatement = "Id <= @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Null()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Name == null);

            var expectStatement = "名前 is null";
            IDictionary<string, object> expectParameter = new ExpandoObject();

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void 非Null()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Name != null);

            var expectStatement = "名前 is not null";
            IDictionary<string, object> expectParameter = new ExpandoObject();

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void And()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id > 1 && x.Name == "xin9le");

            var expectStatement = "Id > @p0 and 名前 = @p1";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Or()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id > 1 || x.Name == "xin9le");

            var expectStatement = "Id > @p0 or 名前 = @p1";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void AndOr1()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id > 1 && x.Name == "xin9le" || x.Age <= 30);

            var expectStatement = "(Id > @p0 and 名前 = @p1) or Age <= @p2";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");
            expectParameter.Add("p2", 30);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void AndOr2()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id > 1 && (x.Name == "xin9le" || x.Age <= 30));

            var expectStatement = "Id > @p0 and (名前 = @p1 or Age <= @p2)";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");
            expectParameter.Add("p2", 30);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void AndOr3()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => x.Id > 1 || x.Name == "xin9le" && x.Age <= 30);

            var expectStatement = "Id > @p0 or (名前 = @p1 and Age <= @p2)";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");
            expectParameter.Add("p2", 30);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void AndOr4()
        {
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => (x.Id > 1 || x.Name == "xin9le") && x.Age <= 30);

            var expectStatement = "(Id > @p0 or 名前 = @p1) and Age <= @p2";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", 1);
            expectParameter.Add("p1", "xin9le");
            expectParameter.Add("p2", 30);

            actual.Parameter.Is(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Contains()
        {
            var value = Enumerable.Range(0, 3).Cast<object>().ToArray();
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => value.Contains(x.Id));

            var expectStatement = "Id in @p0";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", value);

            actual.Parameter.IsStructuralEqual(expectParameter);
            actual.Statement.Is(expectStatement);
        }


        [TestMethod]
        public void Contains1000件超()
        {
            var value1 = Enumerable.Range(0, 1000).Cast<object>().ToArray();
            var value2 = Enumerable.Range(1000, 234).Cast<object>().ToArray();
            var value  = value1.Concat(value2);
            var actual = PredicateSql.From<Person>(DbKind.SqlServer, x => value.Contains(x.Id));

            var expectStatement = "Id in @p0 or Id in @p1";
            IDictionary<string, object> expectParameter = new ExpandoObject();
            expectParameter.Add("p0", value2);
            expectParameter.Add("p1", value1);

            actual.Parameter.IsStructuralEqual(expectParameter);
            actual.Statement.Is(expectStatement);
        }
    }
}