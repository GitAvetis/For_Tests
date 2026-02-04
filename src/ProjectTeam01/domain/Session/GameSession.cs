using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Session;

/// Центральный агрегат для управления игровым процессом.
/// Реализация разнесена по partial-файлам для читаемости (без изменения логики).
internal partial class GameSession
{
    private Level _currentLevel;
    private Hero _player;
    private readonly Random _random;
    private readonly LevelGenerator _levelGenerator;
    private readonly EntityGenerator _entityGenerator;
    private LevelMapQuery _mapQuery;
    private readonly GameStatistics _statistics;
    private bool _gameCompleted;

    public Level CurrentLevel => _currentLevel;
    public Hero Player => _player;
    public int CurrentLevelNumber => _currentLevel.LevelNumber;
    public GameStatistics Statistics => _statistics;
    public bool IsGameCompleted => _gameCompleted;

    /// Конструктор для создания НОВОЙ игры.
    /// Создает новую статистику, начиная с указанного уровня.
    public GameSession(Level level, Hero player, int levelNumber = 1)
    {
        _currentLevel = level ?? throw new ArgumentNullException(nameof(level));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _currentLevel.LevelNumber = levelNumber;
        _random = new Random();
        _levelGenerator = new LevelGenerator();
        _entityGenerator = new EntityGenerator();
        _mapQuery = new LevelMapQuery(_currentLevel);
        _statistics = new GameStatistics(levelNumber);

        // Добавляем игрока на уровень, если его там еще нет
        if (!_currentLevel.Entities.Contains(_player))
        {
            _currentLevel.AddEntity(_player);
        }
    }

    /// Конструктор для ЗАГРУЗКИ игры из сохранения.
    /// Использует восстановленную статистику (обязательно должна быть загружена из файла).
    public GameSession(Level level, Hero player, int levelNumber, GameStatistics statistics)
    {
        _currentLevel = level ?? throw new ArgumentNullException(nameof(level));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        _currentLevel.LevelNumber = levelNumber;
        _random = new Random();
        _levelGenerator = new LevelGenerator();
        _entityGenerator = new EntityGenerator();
        _mapQuery = new LevelMapQuery(_currentLevel);

        if (!_currentLevel.Entities.Contains(_player))
        {
            _currentLevel.AddEntity(_player);
        }
    }
}
