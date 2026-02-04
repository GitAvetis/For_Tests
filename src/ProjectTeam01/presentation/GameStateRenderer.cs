using Mindmagma.Curses;
using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Items;
using ProjectTeam01.presentation.Frontend;
using ProjectTeam01.presentation.ViewModels;


namespace ProjectTeam01.presentation;
internal static class GameStateRenderer
{
    /// Отрисовывает состояние игры в консоль
    public static void RenderHandler(GameStateViewModel viewModel, nint stdscr, GameController controller, char[,] map)
    {
        NCurses.GetMaxYX(stdscr, out int maxY, out int maxX);
        NCurses.Clear();
        if (controller.Session.IsGameOver() || controller.Session.IsGameCompleted)
        {
            GameOverScreen.RenderGameOverScreen(stdscr, controller);
            return;
        }

        switch (controller.CurrentInputMode)
        {
            // case InputMode.MainMenu:
            //     mainMenu.RenderMenu();
            // break;
            case InputMode.Normal:
                RenderMap(viewModel, maxY, maxX, map);
                break;
            case InputMode.ElixirMenu:
                RenderMenu("Elexirs", viewModel.InventoryElixirs, maxY, maxX);
                break;
            case InputMode.ScrollMenu:
                RenderMenu("Scrolls", viewModel.InventoryScrolls, maxY, maxX);
                break;
            case InputMode.WeaponMenu:
                RenderMenu("Weapons", viewModel.InventoryWeapons, maxY, maxX);
                break;
            case InputMode.FoodMenu:
                RenderMenu("Food", viewModel.InventoryFood, maxY, maxX);
                break;
        }
        NCurses.Refresh();
    }
    private static void RenderMap(GameStateViewModel viewModel, int maxY, int maxX, char[,] map)
    {
        // Отрисовываем коридоры (сначала, чтобы они были под комнатами)
        foreach (var corridor in viewModel.Level.Corridors)
        {
            RenderCorridor(map, corridor);
        }
        // Отрисовываем комнаты
        foreach (var room in viewModel.Level.Rooms)
        {
            RenderRoom(map, room);
        }

        // Отрисовываем двери
        foreach (var room in viewModel.Level.Rooms)
        {
            RenderDoors(map, room);
        }

        // Отрисовываем стартовую позицию
        if (viewModel.Level.StartPosition.X > 0 && viewModel.Level.StartPosition.Y > 0)
        {
            map[viewModel.Level.StartPosition.Y, viewModel.Level.StartPosition.X] = 'S';
        }

        // Отрисовываем конечную позицию
        if (viewModel.Level.ExitPosition.X > 0 && viewModel.Level.ExitPosition.Y > 0)
        {
            map[viewModel.Level.ExitPosition.Y, viewModel.Level.ExitPosition.X] = 'E';
        }

        // Отрисовываем сущности (игрок, враги, предметы)
        RenderEntities(map, viewModel);

        // Выводим карту в консоль
        // PrintMap(map);
        PrintMapNCurses(map, maxY, maxX, viewModel);
    }


    /// Отрисовывает сущности на карте
    private static void RenderEntities(char[,] map, GameStateViewModel viewModel)
    {
        // Отрисовываем игрока
        var playerPos = viewModel.Player.Position;
        if (IsValidPosition(playerPos, map))
        {
            map[playerPos.Y, playerPos.X] = '@'; // Игрок
        }

        // Отрисовываем врагов
        foreach (var enemy in viewModel.Enemies)
        {
            if (enemy.IsDead) continue; // Пропускаем мертвых врагов
            if (enemy.IsInvisible == true) continue; // Пропускаем невидимых призраков

            var pos = enemy.Position;
            if (!IsValidPosition(pos, map)) continue;

            // Если на этой позиции уже есть игрок или выход, не перезаписываем
            if (map[pos.Y, pos.X] == '@' || map[pos.Y, pos.X] == 'E')
                continue;

            // Символ врага по типу
            char enemySymbol;
            if(enemy.EnemyType == EnemyTypeEnum.Mimic && enemy.IsTriggered)
            {
                enemySymbol = GetItemSymbol (enemy.MimicRepresentation);
            }
            else enemySymbol = GetEnemySymbol(enemy.EnemyType);
            map[pos.Y, pos.X] = enemySymbol;

        }

        // Отрисовываем предметы
        foreach (var item in viewModel.Items)
        {
            var pos = item.Position;
            if (!IsValidPosition(pos, map)) continue;

            // Если на этой позиции уже есть игрок, враг или выход, не перезаписываем
            if (map[pos.Y, pos.X] == '@' || map[pos.Y, pos.X] == 'E' ||
                IsEnemySymbol(map[pos.Y, pos.X]))
                continue;

            // Символ предмета по типу
            char itemSymbol = GetItemSymbol(item.Type);
            // Не перезаписываем пол, если это не пустое место
            if (map[pos.Y, pos.X] == '.' || map[pos.Y, pos.X] == ' ')
            {
                map[pos.Y, pos.X] = itemSymbol;
            }
        }
        NCurses.Refresh();
    }

    /// Получить символ врага по типу
    private static char GetEnemySymbol(EnemyTypeEnum enemyType)
    {
        return enemyType switch
        {
            EnemyTypeEnum.Zombie => 'z',
            EnemyTypeEnum.Vampire => 'v',
            EnemyTypeEnum.Ghost => 'g',
            EnemyTypeEnum.Ogre => 'O',
            EnemyTypeEnum.Snake => 's',
            EnemyTypeEnum.Mimic => 'm',
            _ => '?'
        };
    }

    /// Получить символ предмета
    private static char GetItemSymbol(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Treasure => '$',
            ItemType.Food => 'F',
            ItemType.Elixir => 'e',
            ItemType.Scroll => '?',
            ItemType.Weapon => 'W',
            _ => '?'
        };
    }

    /// Проверить, является ли символ символом врага
    private static bool IsEnemySymbol(char symbol)
    {
        return symbol == 'z' || symbol == 'v' || symbol == 'g' ||
               symbol == 'O' || symbol == 's' || symbol == 'm';
    }

    /// Проверить валидность позиции для отрисовки
    private static bool IsValidPosition(Position pos, char[,] map)
    {
        return pos.Y >= 0 && pos.Y < map.GetLength(0) &&
               pos.X >= 0 && pos.X < map.GetLength(1);
    }

    /// Отрисовывает комнату на карте
    private static void RenderRoom(char[,] map, RoomViewModel room)
    {
        int top = room.TopLeft.Y;
        int left = room.TopLeft.X;
        int bottom = room.BottomRight.Y;
        int right = room.BottomRight.X;

        // Отрисовываем стены и пол
        for (int y = top; y <= bottom; y++)
        {
            for (int x = left; x <= right; x++)
            {
                // Проверяем границы
                if (y < 0 || y >= GenerationConstants.MapHeight ||
                    x < 0 || x >= GenerationConstants.MapWidth)
                    continue;

                // Стены
                if (y == top || y == bottom || x == left || x == right)
                {
                    map[y, x] = '#';
                }
                // Пол
                else
                {
                    // Если это не коридор, рисуем пол
                    if (map[y, x] == ' ' || map[y, x] == '.')
                    {
                        map[y, x] = '.';
                    }
                }
            }
        }
    }

    /// Отрисовывает коридор на карте
    private static void RenderCorridor(char[,] map, CorridorViewModel corridor)
    {
        if (corridor.Cells == null || corridor.Cells.Count == 0)
            return;

        // Отрисовываем все клетки коридора
        foreach (var cell in corridor.Cells)
        {
            // Проверяем границы
            if (cell.Y >= 0 && cell.Y < GenerationConstants.MapHeight &&
                cell.X >= 0 && cell.X < GenerationConstants.MapWidth)
            {
                // Рисуем коридор ('.' для пола коридора)
                if (map[cell.Y, cell.X] == ' ')
                {
                    map[cell.Y, cell.X] = '.';
                }
            }
        }
    }

    /// Отрисовывает двери комнаты
    private static void RenderDoors(char[,] map, RoomViewModel room)
    {
        if (room.Doors == null)
            return;

        foreach (var door in room.Doors)
        {
            // Проверяем, что дверь инициализирована
            if (door.X > 0 && door.Y > 0)
            {
                // Проверяем границы
                if (door.Y >= 0 && door.Y < GenerationConstants.MapHeight &&
                    door.X >= 0 && door.X < GenerationConstants.MapWidth)
                {
                    // Дверь обозначаем символом '+'
                    // Но только если это не стартовая или конечная позиция
                    if (map[door.Y, door.X] != 'S' && map[door.Y, door.X] != 'E')
                    {
                        map[door.Y, door.X] = '+';
                    }
                }
            }
        }
    }

    private static void PrintMapNCurses(char[,] map, int maxY, int maxX, GameStateViewModel viewModel)
    {
        // Получаем размеры терминала

        int mapHeight = map.GetLength(0);
        int mapWidth = map.GetLength(1);

        // Ограничиваем размеры карты размером окна (без выхода за границы)
        int winHeight = Math.Min(mapHeight, maxY - 1);
        int winWidth = Math.Min(mapWidth, maxX - 1);
        int startY = Math.Max(0, (maxY - winHeight) / 2);  // Центрируем по вертикали
        int startX = Math.Max(0, (maxX - winWidth) / 2);
        // Создаем подокно строго для карты
        nint win = NCurses.NewWindow(winHeight, winWidth, startY, startX);

        // Отрисовка карты
        for (int y = 0; y < winHeight; y++)
        {
            for (int x = 0; x < winWidth; x++)
            {
                char c = map[y, x];

                // Заменяем null-символы на пробел
                if (c == '\0') c = ' ';
                uint color = GetColorForChar(c);
                // Безопасно пишем символ
                NCurses.WindowMove(win, y, x);
                if (color != 0)
                    NCurses.WindowAttributeOn(win, color);
                try
                {
                    NCurses.WindowAddChar(win, c);
                }
                catch (DotnetCursesException)
                {
                    // Игнорируем ошибку, чтобы программа не падала
                }
                if (color != 0)
                    NCurses.WindowAttributeOff(win, color);
            }
        }

        // Обновляем окно
        NCurses.WindowRefresh(win);

        if (winHeight + 1 > maxY || winWidth + 1 > maxX)
        {
            NCurses.Move(winHeight, 0);
            NCurses.AddString("Terminal is to small for stats");
            return;
        }
        else
            PrintStats(startY + winHeight, startX, winWidth, viewModel);

        // int legendStartY = winHeight+2; 
        //PrintLegend(legendStartY,maxX,maxY);
    }
    private static void PrintLegend(int legendStartY, int maxX, int maxY)
    {
        string[] legendLines =
        {
            "Legend:",
            "  # = Wall",
            "  . = Floor",
            "  + = Door",
            "  S = Start",
            "  E = Exit",
            "",
            "Entities:",
            "  @ = Player",
            "  z = Zombie",
            "  v = Vampire",
            "  g = Ghost",
            "  O = Ogre",
            "  s = Snake",
            "  m = Mimic",
            "  $ = Treasure",
            "  F = Food",
            "  e = Elixir",
            "  ? = Scroll",
            "  W = Weapon"
        };

        for (int i = 0; i < legendLines.Length; i++)
        {
            int y = legendStartY + i;
            if (y >= maxY) break; // не выходим за экран
            string line = legendLines[i];
            if (line.Length > maxX) line = line.Substring(0, maxX - 1); // обрезаем, если длиннее
            NCurses.Move(y, 0);
            NCurses.AddString(line);
        }
    }

    private static void PrintStats(int winHeight, int startX, int winWidth, GameStateViewModel viewModel)
    {
        int y = winHeight + 1;
        var player = viewModel.Player;

        string[] statsInfo =
        {
            $"Lvl: {viewModel.CurrentLevelNumber}",
            $"Agil: {player.Agility}",
            $"Str: {player.Strength}",
            $"HP: {player.Health}/{player.MaxHealth}",
            $"Total gold: {viewModel.TotalGold}"
        };

        int partsWith = winWidth / statsInfo.Length;
        for (int i = 0; i < statsInfo.Length; i++)
        {
            string stat = statsInfo[i];
            int x = partsWith * i + (partsWith - stat.Length) / 2 + startX;
            if (x < startX) x = startX;
            if (x + stat.Length >= startX + winWidth) continue;
            NCursesMethods.Print(stat, y, x);
        }
    }
    private static uint GetColorForChar(char c)
    {
        switch (c)
        {
            case '#':
            case '.':
            case '@':
            case 'g':
            case 's':
            case '?':
            case ' ':
            case '+':
                return NCurses.ColorPair(UiColors.White);
            case 'z':
            case 'S':
                return NCurses.ColorPair(UiColors.Green);
            case 'O':
            case 'F':
                return NCurses.ColorPair(UiColors.Yellow);
            case 'v':
            case '$':
            case 'E':
                return NCurses.ColorPair(UiColors.Red);
            case 'e':
            case 'W':
                return NCurses.ColorPair(UiColors.Blue);
            default:
                return 0;
        }

    }

    private static void RenderMenu(string title, List<InventoryItemViewModel> items, int maxY, int maxX)
    {
        int y = maxY / 2;
        int titleX = Math.Max(0, (maxX - title.Length) / 2);
        NCursesMethods.Print(title, y, titleX);
        if (items.Count <= 0)
        {
            string message = $"There is no items of {title} type in your invetory ";
            int messageX = Math.Max(0, (maxX - message.Length) / 2);
            NCursesMethods.Print(message, y + 1, messageX);
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            int itemY = y + 1 + i;
            if (itemY >= maxY) break;
            NCursesMethods.Print($"{i + 1}. {items[i].DisplayName}", itemY, titleX);
        }
    }


}

