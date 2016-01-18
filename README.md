# DeclarativeSql

This library provides attribute-based table mapping and simple database access. It mainly contains following features.

* Unified connection to the some databases (SQL Server / Oracle / MySQL etc.)
* Easy transaction w/o `TransactionScope` 
* O/R mapping information as meta data
* Very simple SQL generation
* Super easy CRUD access based Dapper




## Database connection

`IDbConnection` is able to be created through `DbProvider` static class like following. Until now when you create `IDbConnection` using `DProviderFactory`, you must specify invariant name of target database provider as string. Now this feature allows you to specify **enum-based** kind of database instead of string-based one.

```cs
using (var connection = DbProvider.CreateConnection(DbKind.SqlServer, "ConnectionString"))
{
    connection.Open();
    //--- do something using IDbConnection
}
```

If you want to use this feature, you must add `DbProviderFactory` into .config file (ex. `machine.config` / `app.config` / `web.config`) like following.

```xml
<system.data>
    <DbProviderFactories>
        <add name="SqlClient Data Provider"
             invariant="System.Data.SqlClient"
             description=".Net Framework Data Provider for SqlServer"
             type="System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
    </DbProviderFactories>
</system.data>
```



## Transaction

This library provides `IDbTransaction`-based disposable transaction is similar to `TransactionScope`.

```cs
using (var connection = DbProvider.CreateConnection(DbKind.SqlServer, "ConnectionString"))
using (var transaction = connection.StartTransaction())  //--- begin transaction and open connection automatically if closed
{
    //--- do something here
    transaction.Complete();  //--- mark transaction completed
}
//--- commit or rollback on transaction.Dispose()
```



## O/R mapping information

This library is inspired by Entity Framework also provides attribute-based database mapping. Following code is its sample.

```cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeclarativeSql.Annotations;

namespace SampleApp
{
    [Table("Person", Schema = "dbo")]  //--- table name
    public class Person
    {
        [Key]  //--- primary key
        [AutoIncrement]  //--- automatic identity
        public int Id { get; set; }

        [Required]            //--- not null
        [Column("FullName")]  //--- column name
        public string Name { get; set; }

        [Required]
        [Sequence("AgeSeq", Schema = "dbo")]  //--- if you wanna use sequence (ex. ORACLE)
        public int Age { get; set; }

        [NotMapped]  //--- don't allow mapping
        public int Sex { get; set; }
    }
}
```

This feature provides meta data of mapping information using those annotations. You can get many information like following.

```cs
public sealed class TableMappingInfo
{
    public Type Type { get; }      //--- type of mapping class
    public string Schema { get; }  //--- schema name of target table
    public string Name { get; }    //--- target table name
    public IReadOnlyList<ColumnMappingInfo> Columns { get; }
}

public sealed class ColumnMappingInfo
{
    public string PropertyName { get; }  //--- property name
    public Type PropertyType { get; }    //--- type of property
    public string ColumnName { get; }    //--- target column name
    public DbType ColumnType { get; }    //--- type of target column
    public bool IsPrimaryKey { get; }    //--- primary key (or not)
    public bool IsNullable { get; }      //--- nullable column (or not)
    public bool IsAutoIncrement { get; } //--- automatic identity column (or not)
    public SequenceMappingInfo Sequence { get; }  //--- sequence information (if property has SequenceAttribute)
}

public sealed class SequenceMappingInfo
{
    public string Schema { get; }  //--- schema name of target sequence
    public string Name { get; }    //--- target sequence name
}
```



## SQL generation

This library also provides automatic sql generation feature using above meta data. You can get very simple and typical sql using `PrimitiveSql` static class. Of course it's completely type-safe.

```cs
//--- query all records (specified column only)
var sql = PrimitiveSql.CreateSelect<Person>(x => x.Id, x => x.Name);

/*
select
    Id as Id,
    FullName as Name
from dbo.Person
*/
```

```cs
//--- insert record to SQL Server
var sql = PrimitiveSql.CreateInsert<Person>(DbKind.SqlServer);

/*
insert into dbo.Person
(
    FullName,
    Age
)
values
(
    @Name,
    next value for dbo.SampleSeq
)
*/
```

```cs
//--- update all records (specifed column only)
var sql = PrimitiveSql.CreateUpdate<Person>(DbKind.SqlServer, x => new { x.Name, x.Age });

/*
update dbo.Person
set
    FullName = @Name,
    Age = @Age
*/
```

`PrimitiveSql` static class also provides some other overload functions and `Count` / `Delete` / `Truncate` methods and so on.




## Dapper integration

This library automates typical CRUD operations completely using above sql generation feature and Dapper. By using expression tree, you can specify target column and filter records. These are all type safe.

```cs
using (var connection = DbProvider.CreateConnection(DbKind.SqlServer, "ConnectionString"))
{
    connection.Open();

    var p1 = connection.Select<Person>();                                            //--- query all records
    var p2 = connection.Select<Person>(x => x.Id, x => x.Name);                      //--- query all records (specified column only)
    var p3 = connection.Select<Person>(x => x.Id == 3);                              //--- query 'ID = 3' records only
    var p4 = connection.Select<Person>(x => x.Id == 3, x => new { x.Id, x.Name } );  //--- query 'ID = 3' records and specified column only

    var p5 = connection.Insert(new Person { Name = "xin9le", Age = 30 });  //--- insert specified data
    var p6 = connection.Insert(new []  //--- can insert collection
    {
        new Person { Name = "yoshiki", Age= 49, },
        new Person { Name = "suzuki",  Age= 30, },
        new Person { Name = "anders",  Age= 54, },
    });
    
    var p7 = connection.BulkInsert(new []  //--- super easy bulk insert
    {
        new Person { Id = 1, Name = "yoshiki", Age= 49, },
        new Person { Id = 2, Name = "suzuki",  Age= 30, },
        new Person { Id = 3, Name = "anders",  Age= 54, },
    });
    
    var p8 = connection.InsertAndGet(new Person { Name = "xin9le", Age = 30 });  //--- insert and get generated auto increment id

    var p9 = connection.Update(new Person  //--- update records which is matched specified condition
    {
        Name = "test",
        Age = 23
    }, x => x.Age == 30);

    var p10 = connection.Delete<Person>();                  //--- delete all records
    var p11 = connection.Delete<Person>(x => x.Age != 30);  //--- delete records which is matched specified condition

    var p12 = connection.Truncate<Person>();  //--- truncate table

    var p13 = connection.Count<Person>();                       //--- count all records
    var p14 = connection.Count<Person>(x => x.Name == "test");  //--- count records which is matched specified condition
}
```

Because these method names are directly described the CRUD operations, you can understand and use them easily. These functionally methods provides not only `IDbConnection` but also `IDbTransaction`.




## Obsolete

* Some methods (ex. Select / Update) which had `params` arguments are obsoleted. It has been impaired function scalability and comfortable writing.
* Unmanaged ODP.NET is obsoleted, because now this library supports managed ODP.NET.




## Breaking Changes (v0.1 -> v0.2)

* Past version `DbKind.Oracle` is renamed to `DbKind.UnmanagedOracle`. The brand new `DbKind.Oracle` has been defined for managed ODP.NET.
* Add `timeout` argument to `Insert` method.
* `StartTransaction` method's return value has been changed. However no problem if you receive it using `var`.
* `ColumnMappingInfo.IsIdentity` property has been renamed to `ColumnMappingInfo.IsAutoIncrement`.
* Some utility class has been hidden.
    * `ExpandoObjectExtensions` class
    * `AccessorCache` class
    * `PredicateElement` class
    * `PredicateElementExtensions` class
    * `PredicateOperator` class
    * `PredicateOperatorExtensions` class
    * `PredicateParser` class




## Installation

Getting started from downloading NuGet packages.

```
PM> Install-Package DeclarativeSql.Core
PM> Install-Package DeclarativeSql.Dapper
```




## License

This library is provided under [MIT License](http://opensource.org/licenses/MIT).





## Author

Takaaki Suzuki (a.k.a [@xin9le](https://twitter.com/xin9le)) is software developer in Japan who awarded Microsoft MVP for Visual Studio and Development Technologies (Visual C#) since July 2012.
