using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Session;
using ProjectTeam01.presentation.Mappers;

namespace ProjectTeam01.domain.generation;

/// Пример использования GameStateRenderer для отрисовки уровня через ViewModels
/// Этот класс можно использовать для тестирования отрисовки
public static class RenderExample
{
    /// Генерирует и отрисовывает уровень через ViewModels
    public static void GenerateAndRender()
    {
        var generator = new LevelGenerator();
        var level = generator.GenerateLevel();

        // Создаем героя и игровую сессию для получения ViewModel
        var hero = new Hero(level.StartPosition.X, level.StartPosition.Y);
        var gameSession = new GameSession(level, hero, level.LevelNumber);

        // Получаем ViewModel и отрисовываем
        var gameState = gameSession.GetGameState();
        var viewModel = GameStateMapper.ToViewModel(gameState);

        // GameStateRenderer.Render(viewModel);

        Console.WriteLine();
        Console.WriteLine($"Level Number: {viewModel.CurrentLevelNumber}");
        Console.WriteLine($"Rooms Count: {viewModel.Level.Rooms.Count}");
        Console.WriteLine($"Corridors Count: {viewModel.Level.Corridors.Count}");
        Console.WriteLine($"Start Position: ({viewModel.Level.StartPosition.X}, {viewModel.Level.StartPosition.Y})");
        Console.WriteLine($"Exit Position: ({viewModel.Level.ExitPosition.X}, {viewModel.Level.ExitPosition.Y})");
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}

