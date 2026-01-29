using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain;
using ProjectTeam01.domain.Session;

namespace ProjectTeam01.datalayer.Mappers;

internal static class GameStatisticsMapper
{
    public static GameStatisticsSave ToSave(GameStatistics statistics)
    {
        return new GameStatisticsSave
        {
            TreasuresCollected = statistics.TreasuresCollected,
            MaxLevelReached = statistics.MaxLevelReached,
            EnemiesDefeated = statistics.EnemiesDefeated,
            FoodConsumed = statistics.FoodConsumed,
            ElixirsConsumed = statistics.ElixirsConsumed,
            ScrollsConsumed = statistics.ScrollsConsumed,
            HitsLanded = statistics.HitsLanded,
            HitsMissed = statistics.HitsMissed,
            HitsTaken = statistics.HitsTaken,
            CellsMoved = statistics.CellsMoved
        };
    }

    public static GameStatistics FromSave(GameStatisticsSave save)
    {
        var statistics = new GameStatistics(save.MaxLevelReached);
        statistics.RestoreFrom(
            save.TreasuresCollected,
            save.MaxLevelReached,
            save.EnemiesDefeated,
            save.FoodConsumed,
            save.ElixirsConsumed,
            save.ScrollsConsumed,
            save.HitsLanded,
            save.HitsMissed,
            save.HitsTaken,
            save.CellsMoved
        );
        return statistics;
    }
}

