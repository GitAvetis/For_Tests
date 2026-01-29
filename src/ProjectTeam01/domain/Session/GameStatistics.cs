namespace ProjectTeam01.domain.Session;

/// Статистика одной попытки прохождения
/// Данные собираются в domain и могут быть сохранены datalayer.

internal sealed class GameStatistics
{
    public int TreasuresCollected { get; private set; }
    public int MaxLevelReached { get; private set; }
    public int EnemiesDefeated { get; private set; }
    public int FoodConsumed { get; private set; }
    public int ElixirsConsumed { get; private set; }
    public int ScrollsConsumed { get; private set; }
    public int HitsLanded { get; private set; }
    public int HitsMissed { get; private set; }
    public int HitsTaken { get; private set; }
    public int CellsMoved { get; private set; }

    public GameStatistics(int startLevel = 1)
    {
        TreasuresCollected = 0;
        MaxLevelReached = startLevel;
    }

    public void RecordLevelReached(int level)
    {
        if (level > MaxLevelReached)
            MaxLevelReached = level;
    }

    public void RecordTreasureCollected(int value)
    {
        if (value > 0)
            TreasuresCollected += value;
    }

    public void RecordEnemyDefeated() => EnemiesDefeated++;
    public void RecordFoodConsumed() => FoodConsumed++;
    public void RecordElixirConsumed() => ElixirsConsumed++;
    public void RecordScrollConsumed() => ScrollsConsumed++;
    public void RecordMove(int cells = 1) { if (cells > 0) CellsMoved += cells; }
    
    /// Записать попытку попадания
    public void RecordPlayerHitAttempt(bool hit)
    {
        if (hit) HitsLanded++;
        else HitsMissed++;
    }

    /// Записать полученный урон
    public void RecordHitTaken() => HitsTaken++;

    /// Восстановить статистику из сохранения
    internal void RestoreFrom(int treasuresCollected, int maxLevelReached, int enemiesDefeated,
        int foodConsumed, int elixirsConsumed, int scrollsConsumed,
        int hitsLanded, int hitsMissed, int hitsTaken, int cellsMoved)
    {
        TreasuresCollected = treasuresCollected;
        MaxLevelReached = maxLevelReached;
        EnemiesDefeated = enemiesDefeated;
        FoodConsumed = foodConsumed;
        ElixirsConsumed = elixirsConsumed;
        ScrollsConsumed = scrollsConsumed;
        HitsLanded = hitsLanded;
        HitsMissed = hitsMissed;
        HitsTaken = hitsTaken;
        CellsMoved = cellsMoved;
    }
}


