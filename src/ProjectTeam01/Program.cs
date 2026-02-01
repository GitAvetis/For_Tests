using Mindmagma.Curses;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Session;
using ProjectTeam01.presentation.Frontend;
using ProjectTeam01.presentation;
namespace ProjectTeam01
{
    internal class Program
    {
        static readonly nint stdscr = NCurses.InitScreen();
        static void Main(string[] args)
        {
            try
            {
                NCursesMethods.Init(stdscr);
                // Инициализация игры
                InitializeGame();
            }
            finally
            {
                NCursesMethods.Shutdown();
            }
            // Тесты (можно закомментировать для запуска игры)
            // Level(args);
            // Hero hero = new(1, 1);
            // HeroTest(hero);
            // EnemyTest(RandomEnemy());
            //LevelAdvancementTest();
            //JsonTest();
            //StatisticsJsonTest();
        }
        static void InitializeGame()
        {
            // Создаем новую игру через GameInitializer (domain слой)
            var gameSession = GameInitializer.CreateNewGame(levelNumber: 1);
            var controller = new GameController(gameSession);
            bool running = true;
            // Создаем карту
            char[,] map = new char[GenerationConstants.MapHeight, GenerationConstants.MapWidth];
            GameStateRenderer.ActivateColorSystem();
            while (running)
            {
            // Инициализируем карту пробелами
            for (int y = 0; y < GenerationConstants.MapHeight; y++)
            {
                for (int x = 0; x < GenerationConstants.MapWidth; x++)
                {
                    map[y, x] = ' ';
                }
            }  
                var viewModel = controller.GetGameStateViewModel();

                // Отрисовка
                GameStateRenderer.RenderHandler(viewModel, stdscr, controller,map);

                // Получаем ввод
                int key = NCurses.GetChar(); // ждет нажатия клавиши

                // Обработка ввода
                running = controller.HandleInput((char)key);
            }

        }
    //     /// Инициализация и запуск игры
    //     static void InitializeGameStatic()
    //     {
    //         Console.WriteLine("=== ROGUE GAME INITIALIZATION ===");
    //         Console.WriteLine();

    //         // Создаем новую игру через GameInitializer (domain слой)
    //         var gameSession = GameInitializer.CreateNewGame(levelNumber: 1);
    //         var controller = new GameController(gameSession);
    //         var level = gameSession.CurrentLevel;
    //         var hero = gameSession.Player;
            
    //         Console.WriteLine($"Level {level.LevelNumber} generated: {level.Rooms.Count} rooms");
    //         Console.WriteLine($"Player placed at ({hero.Position.X}, {hero.Position.Y})");
    //         Console.WriteLine($"Enemies placed: {level.GetEnemies().Count}");
    //         Console.WriteLine($"Items placed: {level.GetItems().Count}");
    //         Console.WriteLine("Game session created");
    //         Console.WriteLine();

    //         // 4. Получение данных через ViewModels (новый подход)
    //         var viewModel = controller.GetGameStateViewModel();
    //         Console.WriteLine("=== GAME STATE (via ViewModels) ===");
    //         Console.WriteLine($"Player: HP {viewModel.Player.Health}/{viewModel.Player.MaxHealth}, " +
    //                         $"Position ({viewModel.Player.Position.X}, {viewModel.Player.Position.Y})");
    //         Console.WriteLine($"Enemies: {viewModel.Enemies.Count}");
    //         Console.WriteLine($"Items: {viewModel.Items.Count}");
    //         Console.WriteLine($"Exit at: ({viewModel.Level.ExitPosition.X}, {viewModel.Level.ExitPosition.Y})");
    //         Console.WriteLine();
    //         Console.WriteLine("Press Enter to render the level...");
    //         Console.ReadLine();

    //         // 5. Отрисовка уровня с сущностями (через ViewModels)
    //         Console.WriteLine("=== LEVEL RENDERING (via ViewModels) ===");
    //         GameStateRenderer.Render(viewModel,stdscr);
            
    //         Console.WriteLine();
    //         Console.WriteLine("=== GAME STATE DATA (for frontend via ViewModels) ===");
    //         Console.WriteLine($"Level Number: {viewModel.CurrentLevelNumber}");
    //         Console.WriteLine($"Player Position: ({viewModel.Player.Position.X}, {viewModel.Player.Position.Y})");
    //         Console.WriteLine($"Player Stats: HP {viewModel.Player.Health}/{viewModel.Player.MaxHealth}, " +
    //                         $"Strength {viewModel.Player.Strength}, Agility {viewModel.Player.Agility}");
    //         Console.WriteLine($"Enemies Count: {viewModel.Enemies.Count}");
    //         foreach (var enemy in viewModel.Enemies)
    //         {
    //             Console.WriteLine($"  - {enemy.EnemyType} at ({enemy.Position.X}, {enemy.Position.Y}), " +
    //                             $"Dead: {enemy.IsDead}");
    //         }
    //         Console.WriteLine($"Items Count: {viewModel.Items.Count}");
    //         foreach (var item in viewModel.Items)
    //         {
    //             Console.WriteLine($"  - {item.Type} at ({item.Position.X}, {item.Position.Y})");
    //         }
    //         Console.WriteLine();
    //         Console.WriteLine("Game initialized successfully!");
    //         Console.WriteLine("Ready for game loop integration.");

    //         // 6. Сохраняем игру для последующей загрузки в JsonTest
    //         var enemies = level.GetEnemies().ToList();
    //         var items = level.GetItems().ToList();
    //         var statistics = gameSession.Statistics;
            
    //         GameSave save = GameDataService.CreateSave(hero, enemies, items, level, statistics);
    //         string saveFilePath = "game_save.json";
    //         GameDataService.SaveToFile(save, saveFilePath);
    //         Console.WriteLine($"Game saved to {saveFilePath} for testing");

    //         // Чтобы окно консоли не закрывалось сразу при запуске exe двойным кликом
    //         Console.WriteLine();
    //         Console.WriteLine("Press Enter to exit...");
    //         Console.ReadLine();
    //     }



    //     static void Level(string[] args)
    //     {
    //         Console.WriteLine("Dungeon Level Renderer");
    //         Console.WriteLine("=====================");
    //         Console.WriteLine();

    //         RenderExample.GenerateAndRender();
    //     }

    //     /// Тест перехода на следующий уровень с отрисовкой
    //     static void LevelAdvancementTest()
    //     {
    //         Console.WriteLine("=== LEVEL ADVANCEMENT TEST ===");
    //         Console.WriteLine();
    //         Console.WriteLine("Press Enter to start the test...");
    //         Console.ReadLine();

    //         // 1. Создаем игру на уровне 1
    //         var gameSession = GameInitializer.CreateNewGame(levelNumber: 1);
    //         var controller = new GameController(gameSession);
            
    //         Console.WriteLine("=== LEVEL 1 ===");
    //         var viewModel1 = controller.GetGameStateViewModel();
        
    //         GameStateRenderer.Render(viewModel1,stdscr);
    //         Console.WriteLine();
    //         Console.WriteLine("Press Enter to advance to Level 2...");
    //         Console.ReadLine();
    //         // 2. Создаем новый уровень и переходим на него
    //         var levelGenerator = new LevelGenerator();
    //         var nextLevel = levelGenerator.GenerateLevel();
            
    //         int oldLevelNumber = gameSession.CurrentLevelNumber;
    //         Console.WriteLine($"Current Level Number before advancement: {oldLevelNumber}");
            
    //         gameSession.AdvanceToNextLevel(nextLevel);
            
    //         int newLevelNumber = gameSession.CurrentLevelNumber;
    //         Console.WriteLine($"Current Level Number after advancement: {newLevelNumber}");
    //         Console.WriteLine();
           
    //         var viewModel2 = controller.GetGameStateViewModel();
           
    //         Console.WriteLine();
    //         GameStateRenderer.Render(viewModel2,stdscr);
    //         Console.WriteLine();
    //          Console.WriteLine("Press Enter to advance to Level 3...");
    //         Console.ReadLine();
    //         var nextLevel3 = levelGenerator.GenerateLevel();
            
    //         int oldLevelNumber3 = gameSession.CurrentLevelNumber;
    //         Console.WriteLine($"Current Level Number before advancement: {oldLevelNumber3}");
            
    //         gameSession.AdvanceToNextLevel(nextLevel3);
            
    //         int newLevelNumber3 = gameSession.CurrentLevelNumber;
    //         Console.WriteLine($"Current Level Number after advancement: {newLevelNumber3}");
    //         Console.WriteLine();

    //         var viewModel3 = controller.GetGameStateViewModel();
            
    //         GameStateRenderer.Render(viewModel3,stdscr);
    //         Console.WriteLine();
    //         Console.WriteLine("=== TEST COMPLETED ===");
    //         Console.WriteLine();

    //     }

    //     static void JsonTest()
    //     {
    //         Console.WriteLine("=== JSON SAVE/LOAD TEST ===");
    //         Console.WriteLine();
    //         Console.WriteLine("Loading game saved in InitializeGame()...");
    //         Console.WriteLine();

    //         // Загружаем игру, сохраненную в InitializeGame()
    //         string filePath = "game_save.json";
            
    //         if (!File.Exists(filePath))
    //         {
    //             Console.WriteLine($"ERROR: Save file {filePath} not found!");
    //             Console.WriteLine("Make sure InitializeGame() was called first.");
    //             return;
    //         }

    //         // Загружаем игру через GameDataService (получаем GameSession)
    //         var gameSession = GameDataService.LoadGame(filePath);
    //         var controller = new GameController(gameSession);
    //         Console.WriteLine("Game loaded successfully!");
    //         Console.WriteLine();

    //         // Получаем ViewModel для проверки
    //         var viewModel = controller.GetGameStateViewModel();
            
    //         // Проверяем восстановленные данные через ViewModel
    //         Console.WriteLine("=== LOADED GAME DATA ===");
    //         Console.WriteLine($"Hero Position: ({viewModel.Player.Position.X}, {viewModel.Player.Position.Y})");
    //         Console.WriteLine($"Hero HP: {viewModel.Player.Health}/{viewModel.Player.MaxHealth}");
    //         Console.WriteLine($"Enemies count: {viewModel.Enemies.Count}");
    //         Console.WriteLine($"Items count: {viewModel.Items.Count}");
    //         Console.WriteLine($"Level Number: {viewModel.CurrentLevelNumber}");
    //         Console.WriteLine($"Rooms count: {viewModel.Level.Rooms.Count}");
    //         Console.WriteLine($"Corridors count: {viewModel.Level.Corridors.Count}");
    //         Console.WriteLine($"Start Position: ({viewModel.Level.StartPosition.X}, {viewModel.Level.StartPosition.Y})");
    //         Console.WriteLine($"Exit Position: ({viewModel.Level.ExitPosition.X}, {viewModel.Level.ExitPosition.Y})");
    //         Console.WriteLine($"Statistics - Treasures: {controller.GetStatistics().TreasuresCollected}, Enemies Defeated: {controller.GetStatistics().EnemiesDefeated}");
    //         Console.WriteLine();

    //         // Проверяем через ViewModel (как в InitializeGame)
            
    //         Console.WriteLine("=== VERIFICATION VIA VIEWMODEL ===");
    //         Console.WriteLine($"Level Number: {viewModel.CurrentLevelNumber}");
    //         Console.WriteLine($"Player Position: ({viewModel.Player.Position.X}, {viewModel.Player.Position.Y})");
    //         Console.WriteLine($"Player Stats: HP {viewModel.Player.Health}/{viewModel.Player.MaxHealth}, " +
    //                         $"Strength {viewModel.Player.Strength}, Agility {viewModel.Player.Agility}");
    //         Console.WriteLine($"Enemies Count: {viewModel.Enemies.Count}");
    //         foreach (var enemy in viewModel.Enemies)
    //         {
    //             Console.WriteLine($"  - {enemy.EnemyType} at ({enemy.Position.X}, {enemy.Position.Y}), " +
    //                             $"Dead: {enemy.IsDead}");
    //         }
    //         Console.WriteLine($"Items Count: {viewModel.Items.Count}");
    //         foreach (var item in viewModel.Items)
    //         {
    //             Console.WriteLine($"  - {item.Type} at ({item.Position.X}, {item.Position.Y})");
    //         }
    //         Console.WriteLine();
    //         Console.WriteLine("Save/Load test completed successfully!");
    //     }

    //     /// Тест сохранения и загрузки статистики через JSON
    //     static void StatisticsJsonTest()
    //     {
    //         Console.WriteLine("=== STATISTICS JSON SAVE/LOAD TEST ===");
    //         Console.WriteLine();

    //         // 1. Создаем новую статистику и заполняем её данными
    //         var statistics = new GameStatistics(startLevel: 5);
    //         statistics.RecordTreasureCollected(100);
    //         statistics.RecordTreasureCollected(50);
    //         statistics.RecordLevelReached(7);
    //         statistics.RecordEnemyDefeated();
    //         statistics.RecordEnemyDefeated();
    //         statistics.RecordEnemyDefeated();
    //         statistics.RecordFoodConsumed();
    //         statistics.RecordFoodConsumed();
    //         statistics.RecordElixirConsumed();
    //         statistics.RecordScrollConsumed();
    //         statistics.RecordScrollConsumed();
    //         statistics.RecordScrollConsumed();
    //         statistics.RecordPlayerHitAttempt(hit: true);
    //         statistics.RecordPlayerHitAttempt(hit: true);
    //         statistics.RecordPlayerHitAttempt(hit: false);
    //         statistics.RecordHitTaken();
    //         statistics.RecordMove(10);
    //         statistics.RecordMove(5);

    //         Console.WriteLine("=== ORIGINAL STATISTICS ===");
    //         PrintStatistics(statistics);
    //         Console.WriteLine();

    //         // 2. Сохраняем статистику в JSON (создаем минимальный GameSave только со статистикой)
    //         var statisticsSave = GameStatisticsMapper.ToSave(statistics);
    //         var save = new GameSave
    //         {
    //             Statistics = statisticsSave,
    //             Hero = null!, // Для теста не нужны
    //             Enemies = new List<EnemySave>(),
    //             Items = new List<ItemSave>(),
    //             Level = null!, // Для теста не нужны
    //             GameLevel = statistics.MaxLevelReached
    //         };
            
    //         string testFilePath = "test_statistics.json";
    //         GameDataService.SaveToFile(save, testFilePath);
    //         Console.WriteLine($"Statistics saved to {testFilePath}");
    //         Console.WriteLine();

    //         // 3. Загружаем статистику обратно (используем прямой десериализацию для теста)
    //         string jsonString = File.ReadAllText(testFilePath);
    //         var loadedSave = System.Text.Json.JsonSerializer.Deserialize<GameSave>(jsonString) 
    //             ?? throw new Exception("Failed to deserialize test save.");
    //         var loadedStatistics = GameStatisticsMapper.FromSave(loadedSave.Statistics);
            
    //         Console.WriteLine("=== LOADED STATISTICS ===");
    //         PrintStatistics(loadedStatistics);
    //         Console.WriteLine();

    //         // 4. Проверяем, что все данные совпадают
    //         Console.WriteLine("=== VERIFICATION ===");
    //         bool allMatch = true;
            
    //         allMatch &= Verify("TreasuresCollected", statistics.TreasuresCollected, loadedStatistics.TreasuresCollected);
    //         allMatch &= Verify("MaxLevelReached", statistics.MaxLevelReached, loadedStatistics.MaxLevelReached);
    //         allMatch &= Verify("EnemiesDefeated", statistics.EnemiesDefeated, loadedStatistics.EnemiesDefeated);
    //         allMatch &= Verify("FoodConsumed", statistics.FoodConsumed, loadedStatistics.FoodConsumed);
    //         allMatch &= Verify("ElixirsConsumed", statistics.ElixirsConsumed, loadedStatistics.ElixirsConsumed);
    //         allMatch &= Verify("ScrollsConsumed", statistics.ScrollsConsumed, loadedStatistics.ScrollsConsumed);
    //         allMatch &= Verify("HitsLanded", statistics.HitsLanded, loadedStatistics.HitsLanded);
    //         allMatch &= Verify("HitsMissed", statistics.HitsMissed, loadedStatistics.HitsMissed);
    //         allMatch &= Verify("HitsTaken", statistics.HitsTaken, loadedStatistics.HitsTaken);
    //         allMatch &= Verify("CellsMoved", statistics.CellsMoved, loadedStatistics.CellsMoved);

    //         Console.WriteLine();
    //         if (allMatch)
    //         {
    //             Console.WriteLine("✅ ALL STATISTICS MATCH! Test PASSED!");
    //         }
    //         else
    //         {
    //             Console.WriteLine("❌ SOME STATISTICS DON'T MATCH! Test FAILED!");
    //         }
    //         Console.WriteLine();

    //         // Удаляем тестовый файл
    //         if (File.Exists(testFilePath))
    //         {
    //             File.Delete(testFilePath);
    //             Console.WriteLine($"Test file {testFilePath} deleted.");
    //         }
    //     }

    //     static void PrintStatistics(GameStatistics stats)
    //     {
    //         Console.WriteLine($"TreasuresCollected: {stats.TreasuresCollected}");
    //         Console.WriteLine($"MaxLevelReached: {stats.MaxLevelReached}");
    //         Console.WriteLine($"EnemiesDefeated: {stats.EnemiesDefeated}");
    //         Console.WriteLine($"FoodConsumed: {stats.FoodConsumed}");
    //         Console.WriteLine($"ElixirsConsumed: {stats.ElixirsConsumed}");
    //         Console.WriteLine($"ScrollsConsumed: {stats.ScrollsConsumed}");
    //         Console.WriteLine($"HitsLanded: {stats.HitsLanded}");
    //         Console.WriteLine($"HitsMissed: {stats.HitsMissed}");
    //         Console.WriteLine($"HitsTaken: {stats.HitsTaken}");
    //         Console.WriteLine($"CellsMoved: {stats.CellsMoved}");
    //     }

    //     static bool Verify(string fieldName, int expected, int actual)
    //     {
    //         bool match = expected == actual;
    //         string status = match ? "✅" : "❌";
    //         Console.WriteLine($"{status} {fieldName}: expected {expected}, got {actual}");
    //         return match;
    //     }

    }
}
