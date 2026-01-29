using ProjectTeam01.datalayer;
using ProjectTeam01.domain;
using ProjectTeam01.domain.Items;
using ProjectTeam01.domain.Session;
using ProjectTeam01.presentation.Mappers;
using ProjectTeam01.presentation.ViewModels;

namespace ProjectTeam01.presentation;

/// Тонкий контроллер для связи UI и Domain.
/// Переводит ввод пользователя в PlayerAction и вызывает Domain.
internal class GameController
{
    private GameSession _session;
    private bool _running = true;
    private const string ScoreboardFilePath = "scoreboard.json";
    private const string GameSaveFilePath = "game_save.json";

    public bool Running => _running;
    public GameSession Session => _session;

    public GameController(GameSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// Обработать ввод пользователя (клавиша)
    /// Возвращает true, если нужно продолжить игру, false - если выход
    public bool HandleInput(char key)
    {
        PlayerAction? action = null;

        // Движение
        if (key == 'w' || key == 'W')
            action = PlayerAction.CreateMove(_session.Player.Position.X, _session.Player.Position.Y - 1);
        else if (key == 's' || key == 'S')
            action = PlayerAction.CreateMove(_session.Player.Position.X, _session.Player.Position.Y + 1);
        else if (key == 'a' || key == 'A')
            action = PlayerAction.CreateMove(_session.Player.Position.X - 1, _session.Player.Position.Y);
        else if (key == 'd' || key == 'D')
            action = PlayerAction.CreateMove(_session.Player.Position.X + 1, _session.Player.Position.Y);

        // Инвентарь (по ТЗ: h - оружие, j - еда, k - эликсир, e - свиток)
        else if (key == 'h' || key == 'H')
            action = HandleWeaponSelection();
        else if (key == 'j' || key == 'J')
            action = HandleFoodSelection();
        else if (key == 'k' || key == 'K')
            action = HandleElixirSelection();
        else if (key == 'e' || key == 'E')
            action = HandleScrollSelection();

        // Выход
        else if (key == 'q' || key == 'Q' || key == '\x1b') // ESC
            action = PlayerAction.CreateQuit();

        if (action != null)
        {
            // Сохраняем номер уровня до хода для проверки перехода на новый уровень
            int levelBeforeTurn = _session.CurrentLevelNumber;

            _session.ProcessTurn(action);

            // Сохраняем полную игру при переходе на новый уровень (по ТЗ: "После прохождения каждого уровня")
            if (_session.CurrentLevelNumber > levelBeforeTurn)
            {
                SaveFullGame();
            }

            // Проверка выхода из игры (Quit) - сохраняем полную игру перед выходом (по ТЗ: "После перезапуска игры")
            if (action.Type == PlayerActionType.Quit)
            {
                SaveFullGame();
                _running = false;
                return _running;
            }

            // Проверка окончания игры
            if (_session.IsGameOver() || _session.IsGameCompleted)
            {
                // Добавляем попытку в таблицу лидеров при завершении игры
                GameDataService.AddAttemptToScoreboard(_session.Statistics, ScoreboardFilePath);
                _running = false;
            }
        }

        return _running;
    }

    /// Получить представление состояния игры для фронтенда
    public GameStateViewModel GetGameStateViewModel()
    {
        var gameState = _session.GetGameState();
        return GameStateMapper.ToViewModel(gameState);
    }

    /// Получить статистику
    public GameStatistics GetStatistics()
    {
        return _session.Statistics;
    }

    // === Обработка выбора предметов из инвентаря ===

    private PlayerAction? HandleWeaponSelection()
    {
        var weapons = _session.GetPlayerWeapons();
        if (weapons.Count == 0)
            return null; // Нет оружия

        // Показываем список (заглушка - можно заменить на полноценный View)
        ShowItemList("Оружие (0 - снять, 1-9 - экипировать):", weapons, 0);

        var choice = GetNumericInput(0, weapons.Count);
        if (choice == null) return null;

        if (choice == 0)
            return PlayerAction.CreateUnequipWeapon();
        else if (choice > 0 && choice <= weapons.Count)
            return PlayerAction.CreateEquipWeapon(weapons[choice.Value - 1]);

        return null;
    }

    private PlayerAction? HandleFoodSelection()
    {
        var food = _session.GetPlayerFood();
        if (food.Count == 0)
            return null;

        ShowItemList("Еда (1-9 - использовать):", food, 1);
        var choice = GetNumericInput(1, food.Count);
        if (choice > 0 && choice <= food.Count)
            return PlayerAction.CreateUseItem(food[choice.Value - 1]);

        return null;
    }

    private PlayerAction? HandleElixirSelection()
    {
        var elixirs = _session.GetPlayerElixirs();
        if (elixirs.Count == 0)
            return null;

        ShowItemList("Эликсиры (1-9 - использовать):", elixirs, 1);
        var choice = GetNumericInput(1, elixirs.Count);
        if (choice > 0 && choice <= elixirs.Count)
            return PlayerAction.CreateUseItem(elixirs[choice.Value - 1]);

        return null;
    }

    private PlayerAction? HandleScrollSelection()
    {
        var scrolls = _session.GetPlayerScrolls();
        if (scrolls.Count == 0)
            return null;

        ShowItemList("Свитки (1-9 - использовать):", scrolls, 1);
        var choice = GetNumericInput(1, scrolls.Count);
        if (choice > 0 && choice <= scrolls.Count)
            return PlayerAction.CreateUseItem(scrolls[choice.Value - 1]);

        return null;
    }

    // === Вспомогательные методы для отображения (заглушки, можно заменить на View) ===

    private void ShowItemList<T>(string title, IReadOnlyList<T> items, int startIndex) where T : Item
    {
        Console.WriteLine($"\n{title}");
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"  {i + startIndex}. {GetItemName(items[i])}");
        }
        Console.Write("Выберите: ");
    }

    private string GetItemName(Item item)
    {
        return item switch
        {
            Weapon w => $"Оружие: {w.WeaponType} (+{w.StrengthBonus} силы)",
            Food f => $"Еда (+{f.HealthValue} HP)",
            Elixir e => $"Эликсир: {e.ElixirType}",
            Scroll s => $"Свиток: {s.ScrollType}",
            Treasure t => $"Сокровище ({t.Price} золота)",
            _ => item.Type.ToString()
        };
    }

    private int? GetNumericInput(int min, int max)
    {
        var input = Console.ReadLine();
        if (int.TryParse(input, out int choice) && choice >= min && choice <= max)
            return choice;
        return null;
    }

    /// Сохранить полную игру (герой, враги, предметы, уровень, статистика)
    private void SaveFullGame()
    {
        var hero = _session.Player;
        var enemies = _session.CurrentLevel.GetEnemies().ToList();
        var items = _session.CurrentLevel.GetItems().ToList();
        var level = _session.CurrentLevel;
        var statistics = _session.Statistics;

        var save = GameDataService.CreateSave(hero, enemies, items, level, statistics);
        GameDataService.SaveToFile(save, GameSaveFilePath);
    }
}

