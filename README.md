# DeclarativeSql

This library provides attribute-based table mapping and simple database access. It mainly contains following features.

* Unified connection to the some databases (SQL Server / Oracle / MySQL etc.)
* O/R mapping information as meta data
* Very simple SQL generation
* Super easy CRUD access based Dapper




## Database connection

IDbConnection is able to be created through DbProvider static class like following. Until now when you create IDbConnection using DProviderFactory, you must specify invariant name of target database provider as string. Now this feature allows you to specify **enum-based** kind of database instead of string-based one.

```cs
using (var connection = DbProvider.CreateConnection(DbKind.SqlServer, "ConnectionString"))
{
    connection.Open();
    //--- do something using IDbConnection
}
```

If you want to use this feature, you must add DbProviderFactory into .config file (ex. machine.config / app.config / web.config) like following.

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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  //--- automatic identity
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
    public bool IsIdentity { get; }      //--- automatic identity column (or not)
    public SequenceMappingInfo Sequence { get; }  //--- sequence information (if property has SequenceAttribute)
}

public sealed class SequenceMappingInfo
{
    public string Schema { get; }  //--- schema name of target sequence
    public string Name { get; }    //--- target sequence name
}
```



## SQL generation

This library also provides automatic sql generation feature using above meta data. You can get very simple and typical sql using PrimitiveSql static class. Of course it's completely type-safe.

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
var sql = PrimitiveSql.CreateUpdate<Person>(DbKind.SqlServer, x => x.Name);

/*
update dbo.Person
set
    FullName = @Name
*/
```

PrimitiveSql static class also provides some other overload functions and Count / Delete / Truncate methods and so on.




## Dapper integration

This library automates typical CRUD operations completely using above sql generation feature and Dapper. By using expression tree, you can specify target column and filter records. These are all type safe.

```cs
using (var connection = DbProvider.CreateConnection(DbKind.SqlServer, "ConnectionString"))
{
    connection.Open();

    var p1 = connection.Select<Person>();                                        //--- query all records
    var p2 = connection.Select<Person>(x => x.Id, x => x.Name);                  //--- query all records (specified column only)
    var p3 = connection.Select<Person>(x => x.Id == 3);                          //--- query 'ID = 3' records only
    var p4 = connection.Select<Person>(x => x.Id == 3, x => x.Id, x => x.Name);  //--- query 'ID = 3' records and specified column only

    var p5 = connection.Insert(new Person { Name = "xin9le", Age = 30 });  //--- insert specified data
    var p6 = connection.Insert(new[]  //--- can insert collection
    {
        new Person { Name = "yoshiki", Age= 49, },
        new Person { Name = "suzuki",  Age= 30, },
        new Person { Name = "anders",  Age= 54, },
    });

    var p7 = connection.Update(new Person  //--- update records which is matched specified condition
    {
        Name = "test",
        Age = 23
    }, x => x.Age == 30);

    var p8  = connection.Delete<Person>();                  //--- delete all records
    var p9  = connection.Delete<Person>(x => x.Age != 30);  //--- delete records which is matched specified condition

    var p10 = connection.Truncate<Person>();  //--- truncate table

    var p11 = connection.Count<Person>();                       //--- count all records
    var p12 = connection.Count<Person>(x => x.Name == "test");  //--- count records which is matched specified condition
}
```

Because these method names are directly described the CRUD operations, you can understand and use them easily.



## Installation

Getting started from downloading NuGet packages.

```
PM> Install-Package DeclarativeSql.Core
PM> Install-Package DeclarativeSql.Dapper
```





## License

This library is provided under [MIT License](http://opensource.org/licenses/MIT).





## Author

Takaaki Suzuki (a.k.a [@xin9le](https://twitter.com/xin9le)) is software developer in Japan who awarded Microsoft MVP for .NET since July 2012.
