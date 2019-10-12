using System;
using DeclarativeSql.Annotations;



namespace DeclarativeSql.Tests.Models
{
    [Table(DbKind.SqlServer, "Person", Schema = "dbo")]
    public class Person
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }


        [Column(DbKind.SqlServer, "名前")]
        public string Name { get; set; }


        public int Age { get; set; }


        public bool HasChildren { get; set; }


        [CreatedAt("SYSDATETIME()")]
        public DateTimeOffset CreatedAt { get; set; }


        [ModifiedAt]
        [Column(DbKind.MySql, "UpdatedOn")]
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
