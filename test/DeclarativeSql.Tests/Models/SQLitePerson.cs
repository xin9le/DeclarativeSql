using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace DeclarativeSql.Tests.Models
{
    [Table("Person")]  //--- テーブル名の指定
    public class SQLitePerson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [Column("名前")]
        public string Name { get; set; }


        [Required]
        public int Age { get; set; }


        public bool HasChildren { get; set; }


        [NotMapped]
        public int Sex { get; set; }
    }
}