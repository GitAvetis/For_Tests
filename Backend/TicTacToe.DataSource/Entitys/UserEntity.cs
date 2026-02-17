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
        public string Login { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

    }
}
