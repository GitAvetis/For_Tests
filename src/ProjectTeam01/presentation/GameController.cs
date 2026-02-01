using ProjectTeam01.datalayer;
using ProjectTeam01.domain;
using ProjectTeam01.domain.Session;
using ProjectTeam01.presentation.Mappers;
using ProjectTeam01.presentation.ViewModels;
using ProjectTeam01.presentation.Frontend;

namespace ProjectTeam01.presentation;

/// Тонкий контроллер для связи UI и Domain.
/// Переводит ввод пользователя в PlayerAction и вызывает Domain.
internal class GameController
{
    private InputMode _inputMode = InputMode.Normal;
    public InputMode CurrentInputMode => _inputMode;

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
    public PlayerAction? Translate(InputCommand command)
    {
        var pos = _session.Player.Position;

        switch (command.Type)
        {
            case InputCommandType.MoveUp:
                return PlayerAction.CreateMove(pos.X, pos.Y - 1);

            case InputCommandType.MoveDown:
                return PlayerAction.CreateMove(pos.X, pos.Y + 1);

            case InputCommandType.MoveLeft:
                return PlayerAction.CreateMove(pos.X - 1, pos.Y);

            case InputCommandType.MoveRight:
                return PlayerAction.CreateMove(pos.X + 1, pos.Y);

            case InputCommandType.WeaponMenu:
                _inputMode = InputMode.WeaponMenu;
                return null;
                // return HandleWeaponSelection();

            case InputCommandType.FoodMenu:
                _inputMode = InputMode.FoodMenu;
                return null;
                // return HandleFoodSelection();
                
            case InputCommandType.ElixirMenu:
                _inputMode = InputMode.ScrollMenu;
                return null;
                // return HandleElixirSelection();

            case InputCommandType.ScrollMenu:
                _inputMode = InputMode.ScrollMenu;
                return null;
                // return HandleScrollSelection();

            case InputCommandType.Quit:
                return PlayerAction.CreateQuit();
            default:
                return null;
        }
    }

    public bool HandleInput(char key)
    {
        PlayerAction? action = null;

        if (_inputMode == InputMode.Normal)
        {
            var command = InputHandler.Read(key);
            if (command != null)
                action = Translate(command);
        }
        else
        {
            action = HandleMenuInput(key);
        }

        if (action != null)
            ApplyAction(action);

        return _running;
    }
    /// Обработать ввод пользователя (клавиша)
    /// Возвращает true, если нужно продолжить игру, false - если выход
    // public bool HandleInput(char key)
    // {
    //     PlayerAction? action = null;

    //     // Движение
    //     if (key == 'w' || key == 'W')
    //         action = PlayerAction.CreateMove(_session.Player.Position.X, _session.Player.Position.Y - 1);
    //     else if (key == 's' || key == 'S')
    //         action = PlayerAction.CreateMove(_session.Player.Position.X, _session.Player.Position.Y + 1);
    //     else if (key == 'a' || key == 'A')
    //         action = PlayerAction.CreateMove(_session.Player.Position.X - 1, _session.Player.Position.Y);
    //     else if (key == 'd' || key == 'D')
    //         action = PlayerAction.CreateMove(_session.Player.Position.X + 1, _session.Player.Position.Y);

    //     // Инвентарь (по ТЗ: h - оружие, j - еда, k - эликсир, e - свиток)
    //     else if (key == 'h' || key == 'H')
    //         action = HandleWeaponSelection();
    //     else if (key == 'j' || key == 'J')
    //         action = HandleFoodSelection();
    //     else if (key == 'k' || key == 'K')
    //         action = HandleElixirSelection();
    //     else if (key == 'e' || key == 'E')
    //         action = HandleScrollSelection();

    //     // Выход
    //     else if (key == 'q' || key == 'Q' || key == '\x1b') // ESC
    //         action = PlayerAction.CreateQuit();

    //     if (action != null)
    //     {
    //         // Сохраняем номер уровня до хода для проверки перехода на новый уровень
    //         int levelBeforeTurn = _session.CurrentLevelNumber;

    //         _session.ProcessTurn(action);

    //         // Сохраняем полную игру при переходе на новый уровень (по ТЗ: "После прохождения каждого уровня")
    //         if (_session.CurrentLevelNumber > levelBeforeTurn)
    //         {
    //             SaveFullGame();
    //         }

    //         // Проверка выхода из игры (Quit) - сохраняем полную игру перед выходом (по ТЗ: "После перезапуска игры")
    //         if (action.Type == PlayerActionType.Quit)
    //         {
    //             SaveFullGame();
    //             _running = false;
    //             return _running;
    //         }

    //         // Проверка окончания игры
    //         if (_session.IsGameOver() || _session.IsGameCompleted)
    //         {
    //             // Добавляем попытку в таблицу лидеров при завершении игры
    //             GameDataService.AddAttemptToScoreboard(_session.Statistics, ScoreboardFilePath);
    //             _running = false;
    //         }
    //     }

    //     return _running;
    // }

    private void ApplyAction(PlayerAction action)
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
                return;
            }

            // Проверка окончания игры
            if (_session.IsGameOver() || _session.IsGameCompleted)
            {
                // Добавляем попытку в таблицу лидеров при завершении игры
                GameDataService.AddAttemptToScoreboard(_session.Statistics, ScoreboardFilePath);
                _running = false;
            }
        }
    /// Получить представление состояния игры для фронтенда
    private PlayerAction? HandleMenuInput(char key)
    {
        if (key == '\x1b' || key == 'q')
        {
            _inputMode = InputMode.Normal;
            return null;
        }
        switch (_inputMode)
        {
            case InputMode.WeaponMenu:
                return HandleWeaponSelection(key);
            case InputMode.FoodMenu:
                return HandleFoodSelection(key);
            case InputMode.ElixirMenu:
                return HandleElixirSelection(key);
            case InputMode.ScrollMenu:
                return HandleScrollSelection(key);
        }
        return null;
    }
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
        private PlayerAction? HandleWeaponSelection(char key)
    {
        var weapons = _session.GetPlayerWeapons();
        if (weapons.Count == 0)
            return null; // Нет оружия

        if (key == '0')
            return PlayerAction.CreateUnequipWeapon();

          else  if (key >= '1' && key <= '9')
            {
                int index = key - '1';
                if (index < weapons.Count)
                {
                    _inputMode = InputMode.Normal;
                    return PlayerAction.CreateUseItem(weapons[index]);
                }
            }

        return null;
    }

    // private PlayerAction? HandleWeaponSelection()
    // {
    //     _inputMode = InputMode.WeaponMenu;
    //     return null;
        // var weapons = _session.GetPlayerWeapons();
        // if (weapons.Count == 0)
        //     return null; // Нет оружия

        // // Показываем список (заглушка - можно заменить на полноценный View)
        // ShowItemList("Оружие (0 - снять, 1-9 - экипировать):", weapons, 0);

        // var choice = GetNumericInput(0, weapons.Count);
        // if (choice == null) return null;

        // if (choice == 0)
        //     return PlayerAction.CreateUnequipWeapon();
        // else if (choice > 0 && choice <= weapons.Count)
        //     return PlayerAction.CreateEquipWeapon(weapons[choice.Value - 1]);

        // return null;
   // }

    private PlayerAction? HandleFoodSelection(char key)
    {
        var food = _session.GetPlayerFood();
        if (food.Count == 0)
            return null;
         if (key >= '1' && key <= '9')
            {
                int index = key - '1';
                if (index < food.Count)
                {
                    _inputMode = InputMode.Normal;
                    return PlayerAction.CreateUseItem(food[index]);
                }
            }
        return null;
    }

    private PlayerAction? HandleElixirSelection(char key)
    {
        var elixirs = _session.GetPlayerElixirs();
        if (elixirs.Count == 0)
            return null;
            if (key >= '1' && key <= '9')
            {
                int index = key - '1';
                if (index < elixirs.Count)
                {
                    _inputMode = InputMode.Normal;
                    return PlayerAction.CreateUseItem(elixirs[index]);
                }
            }
        return null;
    }

    private PlayerAction? HandleScrollSelection(char key)
    {
        var scrolls = _session.GetPlayerScrolls();
        if (scrolls.Count == 0)
            return null;
        if (key >= '1' && key <= '9')
            {
                int index = key - '1';
                if (index < scrolls.Count)
                {
                    _inputMode = InputMode.Normal;
                    return PlayerAction.CreateUseItem(scrolls[index]);
                }
            }
        return null;
    }
    // private PlayerAction? HandleScrollSelection()
    // {
    //     _inputMode = InputMode.ScrollMenu;
    //     return null;
    //     // var scrolls = _session.GetPlayerScrolls();
    //     // if (scrolls.Count == 0)
    //     //     return null;

    //     // ShowItemList("Свитки (1-9 - использовать):", scrolls, 1);
    //     // var choice = GetNumericInput(1, scrolls.Count);
    //     // if (choice > 0 && choice <= scrolls.Count)
    //     //     return PlayerAction.CreateUseItem(scrolls[choice.Value - 1]);

    //     // return null;
    // }

    // === Вспомогательные методы для отображения (заглушки, можно заменить на View) ===

    // private void ShowItemList<T>(string title, IReadOnlyList<T> items, int startIndex) where T : Item
    // {
    //     Console.WriteLine($"\n{title}");
    //     for (int i = 0; i < items.Count; i++)
    //     {
    //         Console.WriteLine($"  {i + startIndex}. {GetItemName(items[i])}");
    //     }
    //     Console.Write("Выберите: ");
    // }

    // private string GetItemName(Item item)
    // {
    //     return item switch
    //     {
    //         Weapon w => $"Оружие: {w.WeaponType} (+{w.StrengthBonus} силы)",
    //         Food f => $"Еда (+{f.HealthValue} HP)",
    //         Elixir e => $"Эликсир: {e.ElixirType}",
    //         Scroll s => $"Свиток: {s.ScrollType}",
    //         Treasure t => $"Сокровище ({t.Price} золота)",
    //         _ => item.Type.ToString()
    //     };
    // }

    // private int? GetNumericInput(int min, int max)
    // {
    //     var input = Console.ReadLine();
    //     if (int.TryParse(input, out int choice) && choice >= min && choice <= max)
    //         return choice;
    //     return null;
    // }

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

