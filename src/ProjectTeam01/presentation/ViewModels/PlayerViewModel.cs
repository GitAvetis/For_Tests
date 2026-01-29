using ProjectTeam01.domain.generation;

namespace ProjectTeam01.presentation.ViewModels;

/// Представление игрока для отображения на фронтенде
public class PlayerViewModel
{
    public Position Position { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Agility { get; set; }
    public int Strength { get; set; }
    public bool IsSleep { get; set; }
}

