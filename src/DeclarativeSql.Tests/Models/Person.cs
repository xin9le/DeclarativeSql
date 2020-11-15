using System;
using DeclarativeSql.Annotations;



namespace DeclarativeSql.Tests.Models
{
    [Table(DbKind.SqlServer, "Person", Schema = "dbo")]
    public class Person
    {
#pragma warning disable CS8618
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }


        [Column(DbKind.SqlServer, "名前")]
        public string Name { get; set; }


        public int Age { get; set; }


        public bool HasChildren { get; set; }


        [CreatedAt]
        [DefaultValue(DbKind.SqlServer, "SYSDATETIME()")]
        public DateTimeOffset CreatedAt { get; set; }


        [ModifiedAt]
        [Column(DbKind.MySql, "UpdatedOn")]
        [DefaultValue(DbKind.MySql, "SYSDATETIME()")]
        public DateTimeOffset ModifiedAt { get; set; }
#pragma warning restore CS8618
    }
}
