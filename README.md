# DeclarativeSql

This library provides attribute-based table mapping and simple database access. It mainly contains following features.

- Unified connection to the some databases (SQL Server / Oracle / MySQL / SQLite etc.)
- Attribute-based simple SQL generation
- Super easy CRUD access based Dapper
- Automatically set `CreatedAt` / `ModifiedAt` column.
- High availability connection support by master/slave approach


[![Releases](https://img.shields.io/github/release/xin9le/DeclarativeSql.svg)](https://github.com/xin9le/DeclarativeSql/releases)



# Support Platform

- .NET Standard 2.0



# Attribute-based O/R mapping information

DeclarativeSql that is also inspired by Entity Framework provides attribute-based database mapping. Following code is its sample. Generates SQL and performs O/R mapping that based on these attributes and types.


```cs
using System;
using DeclarativeSql.Annotations;

namespace SampleApp
{
    [Table(DbKind.SqlServer, "T_Person", Schema = "dbo")]  // Customize table name per
    public class Person
    {
        [PrimaryKey]  // Primary key constraint
        [AutoIncrement]  // Automatically numbering
        public int Id { get; set; }

        [Unique(0)]  // Unique constraint by index
        public string Email { get; set; }

        [Column(DbKind.SqlServer, "名前")]  // Customize column name
        public string Name { get; set; }

        [AllowNull]  // Nullable
        public int? Age { get; set; }

        [CreatedAt]  // Set datetime when row is inserted
        [DefaultValue(DbKind.SqlServer, "SYSDATETIME()")]
        public DateTimeOffset CreatedOn { get; set; }

        [ModifiedAt]  // Set datetime when row is updated
        [DefaultValue(DbKind.SqlServer, "SYSDATETIME()")]
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
```



# SQL generation

This library also provides automatic sql generation feature using above meta data. You can get very simple and typical sql using `QueryBuilder` class. Of course it's completely type-safe.


```cs
//--- Query records with specified columns that matched specified condition
var sql
    = DbProvider.SqlServer.QueryBuilder
    .Select<Person>(x => new { x.Id, x.Name })
    .Where(x => x.Name == "xin9le")
    .OrderByDescending(x => x.Name)
    .ThenBy(x => x.CreatedOn)
    .Build()
    .Statement;

/*
select
    [Id] as Id,
    [名前] as Name
from [dbo].[T_Person]
where
    [名前] = @p1
order by
    [名前] desc,
    [CreatedOn]
*/
```

```cs
//--- Insert record to SQL Server
var sql
    = DbProvider.SqlServer.QueryBuilder
    .Insert<Person>()
    .Build()
    .Statement;

/*
insert into [dbo].[T_Person]
(
    [Email],
    [名前],
    [Age],
    [CreatedOn],
    [UpdatedOn]
)
values
(
    @Email,
    @Name,
    @Age,
    SYSDATETIME(),
    SYSDATETIME()
)
*/
```

```cs
//--- Update records with specified columns that matched specified condition
var sql
    = DbProvider.SqlServer.QueryBuilder
    .Update<Person>(x => new { x.Name, x.Age })
    .Where(x => x.Age < 35 || x.Name == "xin9le")
    .Build()
    .Statement;

/*
update [dbo].[T_Person]
set
    [名前] = @Name,
    [Age] = @Age,
    [UpdatedOn] = SYSDATETIME()
where
    [Age] < @p1 or [名前] = @p2
*/
```


`QueryBuilder` class also provides some other overload functions and `Count` / `Delete` / `Truncate` methods, and so on.




# Dapper integration

This library automates typical CRUD operations completely using above sql generation feature and Dapper. By using expression tree, you can specify target column and filter records. Provided method names are directly described the CRUD operations, so you can understand and use them easily.


```cs
//--- Query all records
var p1 = connection.Select<Person>();

//--- Query all records with specified columns
var p2 = connection.Select<Person>(x => new { x.Id, x.Name });

//--- Query 'ID = 3' records only
var p3 = connection.Select<Person>(x => x.Id == 3);

//--- Query 'ID = 3' records with specified columns
var p4 = connection.Select<Person>
(
    x => x.Id == 3,
    x => new { x.Id, x.Name }
);
```

```cs
//--- Insert specified data
var p5 = connection.Insert(new Person { Name = "xin9le", Age = 30 });

//--- Insert collection
var p6 = connection.InsertMulti(new []
{
    new Person { Name = "yoshiki", Age= 49, },
    new Person { Name = "suzuki",  Age= 30, },
    new Person { Name = "anders",  Age= 54, },
});

//--- Super easy bulk insert
var p7 = connection.BulkInsert(new []
{
    new Person { Id = 1, Name = "yoshiki", Age= 49, },
    new Person { Id = 2, Name = "suzuki",  Age= 30, },
    new Person { Id = 3, Name = "anders",  Age= 54, },
});

//--- Insert and get generated auto incremented id
var p8 = connection.InsertAndGetId(new Person { Name = "xin9le", Age = 30 });
```

```cs
//--- Update records which is matched specified condition
var p9 = connection.Update
(
    new Person { Name = "test", Age = 23 },
    x => x.Age == 30
);
```

```cs
//--- Delete all records
var p10 = connection.Delete<Person>();

//--- Delete records which is matched specified condition
var p11 = connection.Delete<Person>(x => x.Age != 30);
```

```cs
//--- Truncate table
var p12 = connection.Truncate<Person>();
```

```cs
//--- Count all records
var p13 = connection.Count<Person>();

//--- Count records which is matched specified condition
var p14 = connection.Count<Person>(x => x.Name == "xin9le");
```


These CRUD methods are provided not only synchronous but also asynchronous.



# High availability connection

If you want to create a highly available database configuration, you can use `HighAvailabilityConnection`. This provides the simple
master/slave pattern. High availability can be achieved simply by writing to the master database and reading from the slave database.


```cs
public class FooConnection : HighAvailabilityConnection
{
    public FooConnection()
        : base("ConnectionString-ToMasterServer", "ConnectionString-ToSlaveServer")
    {}

    protected override IDbConnection CreateConnection(string connectionString, AvailabilityTarget target)
        => new SqlConnection(connectionString);
}
```

```cs
using (var connection = new FooConnection())
{
    //--- Read from slave database
    var p = connection.Slave.Select<Person>();

    //--- Write to master database
    connection.Master.Insert(new Person { Name = "xin9le" });
}
```


Of course, by using the same connection string for the master database and for the slave database, a single database environment can be also supported.



# Installation

Getting started from downloading [NuGet](https://www.nuget.org/packages/DeclarativeSql) package.

```
PM> Install-Package DeclarativeSql
PM> Install-Package DeclarativeSql.MicrosoftSqlClient
PM> Install-Package DeclarativeSql.SystemSqlClient
```



# License

This library is provided under [MIT License](http://opensource.org/licenses/MIT).


# Author

Takaaki Suzuki (a.k.a [@xin9le](https://twitter.com/xin9le)) is software developer in Japan who awarded Microsoft MVP for Developer Technologies (C#) since July 2012.
