using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.DataSource.Entitys
{
    public enum GameState
    {
        Waiting,
        PlayerXTurn,
        PlayerOTurn,
        Draw,
        XWin,
        OWin
    }

    [Table("games")]
    public class GameEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        
        [Required]
        public string JsonField { get; set; } = null!;
        
        [Required]
        public int CurrentPlayer { get; set; }

        [Required]
        public int Result { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }

        public Guid? PlayerXId { get; set; }
        public Guid? PlayerOId { get; set; }
        public GameState State { get; set; }
    }
}
