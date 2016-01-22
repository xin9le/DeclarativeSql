using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeclarativeSql.Annotations;



namespace DeclarativeSql.Tests
{
    [Table("Person", Schema = "dbo")]  //--- テーブル名の指定
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [Column("名前")]
        public string Name { get; set; }


        [Required]
        [Sequence("AgeSeq", Schema = "dbo")]
        public int Age { get; set; }


        [NotMapped]
        public int Sex { get; set; }
    }
}