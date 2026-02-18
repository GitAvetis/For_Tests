using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.DataSource.Entitys
{
    [Table("users")]

    public class UserEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Login { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

    }
}
