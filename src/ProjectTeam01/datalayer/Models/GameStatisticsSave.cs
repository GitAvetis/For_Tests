namespace ProjectTeam01.datalayer.Models;

/// Модель сохранения для GameStatistics
internal class GameStatisticsSave
{
    public int TreasuresCollected { get; set; }
    public int MaxLevelReached { get; set; }
    public int EnemiesDefeated { get; set; }
    public int FoodConsumed { get; set; }
    public int ElixirsConsumed { get; set; }
    public int ScrollsConsumed { get; set; }
    public int HitsLanded { get; set; }
    public int HitsMissed { get; set; }
    public int HitsTaken { get; set; }
    public int CellsMoved { get; set; }
}

