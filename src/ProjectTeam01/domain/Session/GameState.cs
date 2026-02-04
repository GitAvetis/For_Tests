using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.domain.Session;

/// Состояние игры для отображения
internal class GameState
{
    public Position PlayerPosition { get; set; }
    public IReadOnlyList<Enemy> Enemies { get; set; } = new List<Enemy>().AsReadOnly();
    public IReadOnlyList<Item> Items { get; set; } = new List<Item>().AsReadOnly();
    public Level LevelGeometry { get; set; } = null!;
    public int PlayerHealth { get; set; }
    public int PlayerMaxHealth { get; set; }
    public int PlayerAgility { get; set; }
    public int PlayerStrength { get; set; }
    public bool PlayerIsSleep { get; set; }
    public int CurrentLevelNumber { get; set; }

    // Инвентарь игрока
    public IReadOnlyList<Weapon> PlayerWeapons { get; set; } = new List<Weapon>().AsReadOnly();
    public IReadOnlyList<Food> PlayerFood { get; set; } = new List<Food>().AsReadOnly();
    public IReadOnlyList<Elixir> PlayerElixirs { get; set; } = new List<Elixir>().AsReadOnly();
    public IReadOnlyList<Scroll> PlayerScrolls { get; set; } = new List<Scroll>().AsReadOnly();
    public int TotalGold { get; set; } = 0;
}

