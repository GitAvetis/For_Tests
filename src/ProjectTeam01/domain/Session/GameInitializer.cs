using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Session;

/// Инициализатор игры - создает новую игровую сессию.
/// Отвечает за генерацию уровня, размещение сущностей и создание GameSession.
internal static class GameInitializer
{
    /// Создать новую игру и вернуть GameSession.
    public static GameSession CreateNewGame(int levelNumber = 1)
    {
        // Генерируем уровень
        var levelGenerator = new LevelGenerator();
        var level = levelGenerator.GenerateLevel();
        level.LevelNumber = levelNumber;

        // Генерируем сущности
        var entityGenerator = new EntityGenerator();
        var hero = entityGenerator.PlacePlayer(level);
        entityGenerator.PlaceEnemies(level, levelNumber);
        entityGenerator.PlaceItems(level, levelNumber);

        // Создаем и возвращаем GameSession (с новой статистикой)
        return new GameSession(level, hero, levelNumber);
    }
}

