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

            case InputCommandType.FoodMenu:
                _inputMode = InputMode.FoodMenu;
                return null;
                
            case InputCommandType.ElixirMenu:
                _inputMode = InputMode.ElixirMenu;
                return null;

            case InputCommandType.ScrollMenu:
                _inputMode = InputMode.ScrollMenu;
                return null;

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
                 return PlayerAction.CreateEquipWeapon(weapons[index]);
                }
            }

        return null;
    }

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

