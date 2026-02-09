using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Session;
//отвечает за логику перехода на следующий уровень
internal partial class GameSession
{
    /// Проверить, находится ли игрок на выходе
    public bool IsPlayerAtExit() => Player.Position.Equals(_currentLevel.ExitPosition);

    /// Проверить, закончена ли игра (игрок мертв)
    public bool IsGameOver() => Player.IsDead;

    public bool IsLastLevel() => _currentLevel.LevelNumber >= 21;

    /// Проверить, нужно ли перейти на следующий уровень
    public bool ShouldAdvanceLevel() => IsPlayerAtExit();

    private void AdvanceToNextLevelInternal()
    {
        int nextLevelNumber = _currentLevel.LevelNumber + 1;
        var next = _levelGenerator.GenerateLevel();
        next.LevelNumber = nextLevelNumber;
        _statistics.RecordLevelReached(nextLevelNumber);

        RecalculateDifficulty();
        _entityGenerator.PlaceExistingHero(next, Player);
        _entityGenerator.PlaceEnemies(next, nextLevelNumber, DifficultyFactor);
        _entityGenerator.PlaceItems(next, nextLevelNumber, DifficultyFactor);

        _currentLevel = next;
        _mapQuery = new LevelMapQuery(_currentLevel);
        _statistics.RecordLevelReached(nextLevelNumber);
        
        // Очищаем туман войны для нового уровня
        _fogOfWar.Clear();
        
        // Инициализируем туман войны для нового уровня - помечаем стартовую комнату как посещенную
        InitializeFogOfWar();
        // _statistics.RecordLevelReached(nextLevelNumber);
    }

    /// Перейти на следующий уровень (внешний вызов)
    public void AdvanceToNextLevel(Level nextLevel)
    {
        if (nextLevel == null) throw new ArgumentNullException(nameof(nextLevel));

        int nextLevelNumber = _currentLevel.LevelNumber + 1;
        nextLevel.LevelNumber = nextLevelNumber;
        _entityGenerator.PlaceExistingHero(nextLevel, Player);
        _entityGenerator.PlaceEnemies(nextLevel, nextLevelNumber, DifficultyFactor);
        _entityGenerator.PlaceItems(nextLevel, nextLevelNumber, DifficultyFactor);

        _currentLevel = nextLevel;
        _mapQuery = new LevelMapQuery(_currentLevel);
        _statistics.RecordLevelReached(nextLevelNumber);
        
        _fogOfWar.Clear();
        
        InitializeFogOfWar();
    }
}


