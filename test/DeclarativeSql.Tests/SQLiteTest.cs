using System;
using System.IO;
using System.Linq;
using Dapper;
using DeclarativeSql;
using DeclarativeSql.Dapper;
using DeclarativeSql.Tests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace DeclarativeSql.Tests
{
    [TestClass]
    public class SQLiteTest
    {
        private string RootFolder { get; set; }
        private string ConnectionString { get; set; }


        [TestInitialize]
        public void Initialize()
        {
            //--- パス設定
            var temp = Environment.GetEnvironmentVariable("TEMP");
            this.RootFolder = Path.Combine(temp, "DeclarativeSql");
            var database = Path.Combine(this.RootFolder, "sample.db");
            if (Directory.Exists(this.RootFolder))
                Directory.Delete(this.RootFolder, true);
            Directory.CreateDirectory(this.RootFolder);

            //--- ConnectionString
            var builder = new SqliteConnectionStringBuilder();
            builder.DataSource = database;
            this.ConnectionString = builder.ToString();

            //--- サンプルデータベース作成
            using (var conn = DbProvider.Sqlite.CreateConnection())
            {
                var sql =
@"create table Person
(
    Id int,
    名前 text,
    Age int,
    HasChildren int
)";
                conn.ConnectionString = this.ConnectionString;
                conn.Open();
                conn.Execute(sql);
            }

            //--- テストデータ挿入
        }


        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(this.RootFolder))
                Directory.Delete(this.RootFolder, true);
        }


        [TestMethod]
        public void Insert()
        {
            using (var conn = DbProvider.Sqlite.CreateConnection())
            {
                conn.ConnectionString = this.ConnectionString;
                var people = new[]
                {
                    new SQLitePerson(){ Name = "xin9le", Age = 32, HasChildren = true, Sex = 0 },
                    new SQLitePerson(){ Name = "YOSHIKI", Age = 51, HasChildren = false, Sex = 0 },
                    new SQLitePerson(){ Name = "Emma", Age = 18, HasChildren = true, Sex = 1 },
                };
                foreach (var x in people)
                    conn.Insert(x);

                conn.Count<SQLitePerson>().Is(3ul);
            }
        }


        [TestMethod]
        public void BulkInsert()
        {
            using (var conn = DbProvider.Sqlite.CreateConnection())
            {
                conn.ConnectionString = this.ConnectionString;
                var people = new[]
                {
                    new SQLitePerson(){ Name = "xin9le", Age = 32, HasChildren = true, Sex = 0 },
                    new SQLitePerson(){ Name = "YOSHIKI", Age = 51, HasChildren = false, Sex = 0 },
                    new SQLitePerson(){ Name = "Emma", Age = 18, HasChildren = true, Sex = 1 },
                };
                conn.BulkInsert(people);

                conn.Count<SQLitePerson>().Is(3ul);
                var selected = conn.Select<SQLitePerson>();

                selected[0].Name.Is(people[0].Name);
                selected[1].Name.Is(people[1].Name);
                selected[2].Name.Is(people[2].Name);

                selected[0].Age.Is(people[0].Age);
                selected[1].Age.Is(people[1].Age);
                selected[2].Age.Is(people[2].Age);

                selected[0].HasChildren.Is(people[0].HasChildren);
                selected[1].HasChildren.Is(people[1].HasChildren);
                selected[2].HasChildren.Is(people[2].HasChildren);
            }
        }


        [TestMethod]
        public void Update()
        {
            using (var conn = DbProvider.Sqlite.CreateConnection())
            {
                conn.ConnectionString = this.ConnectionString;
                var people = new[]
                {
                    new SQLitePerson(){ Name = "xin9le", Age = 32, HasChildren = true, Sex = 0 },
                    new SQLitePerson(){ Name = "YOSHIKI", Age = 51, HasChildren = false, Sex = 0 },
                    new SQLitePerson(){ Name = "Emma", Age = 18, HasChildren = true, Sex = 1 },
                };
                conn.BulkInsert(people);
                conn.Update(new SQLitePerson()
                {
                    Name = "Takaaki",
                    Age = 32,
                    HasChildren = true,
                    Sex = 0,
                }, x => x.Name == "xin9le");

                conn.Count<SQLitePerson>().Is(3ul);
                var selected = conn.Select<SQLitePerson>();

                selected[0].Name.Is("Takaaki");
                selected[1].Name.Is(people[1].Name);
                selected[2].Name.Is(people[2].Name);

                selected[0].Age.Is(people[0].Age);
                selected[1].Age.Is(people[1].Age);
                selected[2].Age.Is(people[2].Age);

                selected[0].HasChildren.Is(people[0].HasChildren);
                selected[1].HasChildren.Is(people[1].HasChildren);
                selected[2].HasChildren.Is(people[2].HasChildren);
            }
        }


        [TestMethod]
        public void Delete()
        {
            using (var conn = DbProvider.Sqlite.CreateConnection())
            {
                conn.ConnectionString = this.ConnectionString;
                var people = Enumerable.Range(0, 10).Select(x => new SQLitePerson() { Name = $"xin9le_{x}", Age = x });
                conn.BulkInsert(people);
                conn.Count<SQLitePerson>().Is(10ul);
                conn.Delete<SQLitePerson>(x => x.Age > 5).Is(4);
                conn.Count<SQLitePerson>().Is(6ul);
                conn.Delete<SQLitePerson>().Is(6);
                conn.Count<SQLitePerson>().Is(0ul);
            }
        }


        [TestMethod]
        public void Truncate()
        {
            using (var conn = DbProvider.Sqlite.CreateConnection())
            {
                conn.ConnectionString = this.ConnectionString;
                var people = Enumerable.Range(0, 10).Select(x => new SQLitePerson() { Name = $"xin9le_{x}", Age = x });
                conn.BulkInsert(people);
                conn.Count<SQLitePerson>().Is(10ul);
                try
                {
                    //--- SQLite is not supported truncate syntax.
                    conn.Truncate<SQLitePerson>();
                }
                catch (Exception ex)
                {
                    ex.GetType().Is(typeof(SqliteException));
                    ex.Message.Is("SQLite Error 1: 'near \"truncate\": syntax error'.");
                }
            }
        }
    }
}