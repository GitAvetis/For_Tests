using System.Linq;
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
        foreach (var corridor in viewModel.Level.Corridors)
        {
            RenderCorridor(map, corridor);
        }
        foreach (var room in viewModel.Level.Rooms)
        {
            if (room.IsVisited)
            {
                RenderRoom(map, room);
            }
        }
        
        foreach (var room in viewModel.Level.Rooms)
        {
            if (room.IsVisited)
            {
                RenderDoors(map, room);
            }
        }
        
        RenderStartPosition(map, viewModel);
        RenderExitPosition(map, viewModel);
        
        RenderEntities(map, viewModel);
        
        PrintMapNCurses(map, maxY, maxX, viewModel);
    }

    
    /// Отрисовывает сущности на карте с учетом тумана войны
    private static void RenderEntities(char[,] map, GameStateViewModel viewModel)
    {
        RenderPlayer(map, viewModel);
        
        var playerPos = viewModel.Player.Position;
        var playerRoom = viewModel.Level.Rooms.FirstOrDefault(r => r.IsCurrentlyInRoom);
        var playerCorridor = viewModel.Level.Corridors.FirstOrDefault(c => 
            c.Cells != null && c.Cells.Contains(playerPos));
        
        RenderEnemies(map, viewModel, playerPos, playerRoom, playerCorridor);
        RenderItems(map, viewModel);
        
        NCurses.Refresh();
    }
    
    /// Отрисовывает игрока на карте
    private static void RenderPlayer(char[,] map, GameStateViewModel viewModel)
    {
        var playerPos = viewModel.Player.Position;
        if (IsValidPosition(playerPos, map))
        {
            map[playerPos.Y, playerPos.X] = '@';
        }
    }
    
    /// Отрисовывает всех врагов на карте
    private static void RenderEnemies(char[,] map, GameStateViewModel viewModel, 
        Position playerPos, RoomViewModel? playerRoom, CorridorViewModel? playerCorridor)
    {
        foreach (var enemy in viewModel.Enemies)
        {
            if (enemy.IsDead || enemy.IsInvisible == true)
                continue;
            
            var pos = enemy.Position;
            if (!IsValidPosition(pos, map))
                continue;
            
            var enemyCorridor = viewModel.Level.Corridors.FirstOrDefault(c => 
                c.Cells != null && c.Cells.Contains(pos));
            
            if (enemyCorridor != null)
            {
                if (ShouldRenderEnemyInCorridor(enemy, playerPos, playerCorridor, enemyCorridor))
                {
                    if (enemy.EnemyType == EnemyTypeEnum.Mimic && !enemy.IsTriggered)
                    {
                        map[pos.Y, pos.X] = GetItemSymbolForMimic(enemy.MimicRepresentation);
                    }
                    else
                    {
                        map[pos.Y, pos.X] = GetEnemySymbol(enemy.EnemyType);
                    }
                }
                continue;
            }
            
            if (ShouldRenderEnemyInRoom(enemy, pos, playerRoom, playerCorridor, viewModel))
            {
                if (enemy.EnemyType == EnemyTypeEnum.Mimic && !enemy.IsTriggered)
                {
                    map[pos.Y, pos.X] = GetItemSymbolForMimic(enemy.MimicRepresentation);
                }
                else
                {
                    map[pos.Y, pos.X] = GetEnemySymbol(enemy.EnemyType);
                }
            }
        }
    }
    
    /// Проверяет, нужно ли отрисовывать врага в коридоре
    private static bool ShouldRenderEnemyInCorridor(EnemyViewModel enemy, Position playerPos, 
        CorridorViewModel? playerCorridor, CorridorViewModel enemyCorridor)
    {
        if (playerCorridor == null || playerCorridor != enemyCorridor)
            return false;
        
        return ArePositionsInSameCorridorSegment(playerCorridor, playerPos, enemy.Position);
    }
    
    /// Проверяет, нужно ли отрисовывать врага в комнате
    private static bool ShouldRenderEnemyInRoom(EnemyViewModel enemy, Position pos, 
        RoomViewModel? playerRoom, CorridorViewModel? playerCorridor, GameStateViewModel viewModel)
    {
        var enemyRoom = viewModel.Level.Rooms.FirstOrDefault(r => 
            pos.X >= r.TopLeft.X && pos.X <= r.BottomRight.X &&
            pos.Y >= r.TopLeft.Y && pos.Y <= r.BottomRight.Y);
        
        if (enemyRoom == null || !enemyRoom.IsVisited)
            return false;
        
        bool isEnemyOnDoor = enemyRoom.Doors != null && 
            enemyRoom.Doors.Any(door => (door.X != 0 || door.Y != 0) && door.X == pos.X && door.Y == pos.Y);
        
        if (isEnemyOnDoor)
        {
            if (enemyRoom.IsCurrentlyInRoom)
                return true;
            
            if (playerCorridor != null && playerCorridor.Points != null)
            {
                return playerCorridor.Points.Any(p => p.X == pos.X && p.Y == pos.Y);
            }
            return false;
        }
        
        if (!enemyRoom.IsCurrentlyInRoom)
            return false;
        
        if (enemyRoom.VisibilityMap.TryGetValue(pos, out bool visible) && !visible)
            return false;
        
        return true;
    }
    
    /// Отрисовывает все предметы на карте
    private static void RenderItems(char[,] map, GameStateViewModel viewModel)
    {
        foreach (var item in viewModel.Items)
        {
            var pos = item.Position;
            if (!IsValidPosition(pos, map))
                continue;
            
            var itemRoom = viewModel.Level.Rooms.FirstOrDefault(r => 
                pos.X >= r.TopLeft.X && pos.X <= r.BottomRight.X &&
                pos.Y >= r.TopLeft.Y && pos.Y <= r.BottomRight.Y);
            
            if (itemRoom == null || !itemRoom.IsVisited || !itemRoom.IsCurrentlyInRoom)
                continue;
            
            bool hasEnemyAtPosition = viewModel.Enemies.Any(e => 
                !e.IsDead && 
                e.IsInvisible != true && 
                e.Position.X == pos.X && 
                e.Position.Y == pos.Y);
            
            if (hasEnemyAtPosition)
                continue;
            
            if (itemRoom.VisibilityMap.TryGetValue(pos, out bool visible) && !visible)
                continue;
            
            if (map[pos.Y, pos.X] == '@' || map[pos.Y, pos.X] == 'E')
                continue;
            
            if (map[pos.Y, pos.X] == '.' || map[pos.Y, pos.X] == ' ' || map[pos.Y, pos.X] == '+')
            {
                map[pos.Y, pos.X] = GetItemSymbol(item.Type);
            }
        }
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
    private static char GetItemSymbolForMimic(MimicsRepresentation? itemType)
    {
        return itemType switch
        {
            MimicsRepresentation.Food => 'F',
            MimicsRepresentation.Elixir => 'e',
            MimicsRepresentation.Scroll => '?',
            MimicsRepresentation.Weapon => 'W',
            _ => '?'
        };
    }
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
    
    /// Проверить, является ли символ символом предмета
    private static bool IsItemSymbol(char symbol)
    {
        return symbol == '$' || symbol == 'F' || symbol == 'e' || 
               symbol == '?' || symbol == 'W';
    }
    
    /// Проверить валидность позиции для отрисовки
    private static bool IsValidPosition(Position pos, char[,] map)
    {
        return pos.Y >= 0 && pos.Y < map.GetLength(0) &&
               pos.X >= 0 && pos.X < map.GetLength(1);
    }
    
    /// Отрисовывает стартовую позицию только если игрок в комнате и позиция видима
    private static void RenderStartPosition(char[,] map, GameStateViewModel viewModel)
    {
        if (viewModel.Level.StartPosition.X > 0 && viewModel.Level.StartPosition.Y > 0)
        {
            var startPos = viewModel.Level.StartPosition;
            bool shouldShow = false;
            
            var startRoom = viewModel.Level.Rooms.FirstOrDefault(r => 
                r.IsVisited && 
                startPos.X >= r.TopLeft.X && startPos.X <= r.BottomRight.X &&
                startPos.Y >= r.TopLeft.Y && startPos.Y <= r.BottomRight.Y);
            
            if (startRoom != null && startRoom.IsCurrentlyInRoom)
            {
                if (startRoom.VisibilityMap.TryGetValue(startPos, out bool visible) && visible)
                {
                    shouldShow = true;
                }
            }
            
            if (shouldShow)
            {
                map[startPos.Y, startPos.X] = 'S';
            }
        }
    }
    
    /// Отрисовывает конечную позицию только если игрок в комнате и позиция видима
    private static void RenderExitPosition(char[,] map, GameStateViewModel viewModel)
    {
        if (viewModel.Level.ExitPosition.X > 0 && viewModel.Level.ExitPosition.Y > 0)
        {
            var exitPos = viewModel.Level.ExitPosition;
            bool shouldShow = false;
            
            var exitRoom = viewModel.Level.Rooms.FirstOrDefault(r => 
                r.IsVisited && 
                exitPos.X >= r.TopLeft.X && exitPos.X <= r.BottomRight.X &&
                exitPos.Y >= r.TopLeft.Y && exitPos.Y <= r.BottomRight.Y);
            
            if (exitRoom != null && exitRoom.IsCurrentlyInRoom)
            {
                if (exitRoom.VisibilityMap.TryGetValue(exitPos, out bool visible) && visible)
                {
                    shouldShow = true;
                }
            }
            
            if (shouldShow)
            {
                map[exitPos.Y, exitPos.X] = 'E';
            }
        }
    }
    
    /// Проверить, находятся ли две позиции в одном и том же сегменте коридора
    private static bool ArePositionsInSameCorridorSegment(CorridorViewModel corridor, Position pos1, Position pos2)
    {
        if (corridor.Points == null || corridor.Points.Count < 2)
        {
            return true;
        }
        
        if (corridor.CellToSegments == null)
            return false;
        
        if (!corridor.CellToSegments.TryGetValue(pos1, out var segmentIndices1))
        {
            return false;
        }
        
        if (!corridor.CellToSegments.TryGetValue(pos2, out var segmentIndices2))
        {
            return false;
        }
        
        return segmentIndices1.Any(segmentIndex => segmentIndices2.Contains(segmentIndex));
    }
    
    /// Отрисовывает комнату на карте с учетом тумана войны
    private static void RenderRoom(char[,] map, RoomViewModel room)
    {
        int top = room.TopLeft.Y;
        int left = room.TopLeft.X;
        int bottom = room.BottomRight.Y;
        int right = room.BottomRight.X;
        
        bool isCurrentlyInRoom = room.IsCurrentlyInRoom;
        
        for (int y = top; y <= bottom; y++)
        {
            for (int x = left; x <= right; x++)
            {
                if (y < 0 || y >= GenerationConstants.MapHeight || 
                    x < 0 || x >= GenerationConstants.MapWidth)
                    continue;
                
                var pos = new Position(x, y);
                
                if (y == top || y == bottom || x == left || x == right)
                {
                    map[y, x] = '#';
                }
                else
                {
                    if (isCurrentlyInRoom)
                    {
                        if (room.VisibilityMap.TryGetValue(pos, out bool visible))
                        {
                            if (!visible)
                            {
                                if (map[y, x] == ' ' || map[y, x] == '.')
                                {
                                    map[y, x] = '.';
                                }
                            }
                        }
                    }
                    else
                    {
                        if (map[y, x] == ' ' || map[y, x] == '.')
                        {
                            map[y, x] = '.'; 
                        }
                    }
                }
            }
        }
    }
    
    /// Отрисовывает коридор на карте с учетом видимости сегментов
    private static void RenderCorridor(char[,] map, CorridorViewModel corridor)
    {
        if (corridor.Cells == null || corridor.Cells.Count == 0)
            return;
        
        if (corridor.Points == null || corridor.Points.Count < 2)
        {
            foreach (var cell in corridor.Cells)
            {
                if (cell.Y >= 0 && cell.Y < GenerationConstants.MapHeight && 
                    cell.X >= 0 && cell.X < GenerationConstants.MapWidth)
                {
                    if (map[cell.Y, cell.X] == ' ')
                    {
                        map[cell.Y, cell.X] = '.';
                    }
                }
            }
            return;
        }
        
        var cellToSegments = corridor.CellToSegments;
        
        if (cellToSegments == null || cellToSegments.Count == 0)
        {
            foreach (var cell in corridor.Cells)
            {
                if (cell.Y >= 0 && cell.Y < GenerationConstants.MapHeight && 
                    cell.X >= 0 && cell.X < GenerationConstants.MapWidth)
                {
                    if (map[cell.Y, cell.X] == ' ')
                    {
                        map[cell.Y, cell.X] = '.';
                    }
                }
            }
            return;
        }
        
        foreach (var cell in corridor.Cells)
        {
            if (cell.Y >= 0 && cell.Y < GenerationConstants.MapHeight && 
                cell.X >= 0 && cell.X < GenerationConstants.MapWidth)
            {
                if (cellToSegments.TryGetValue(cell, out var segmentIndices))
                {
                    bool isVisible = segmentIndices.Any(segmentIndex => 
                        corridor.SegmentVisibility.TryGetValue(segmentIndex, out bool visible) && visible);
                    
                    if (isVisible)
                    {
                        if (map[cell.Y, cell.X] == ' ')
                        {
                            map[cell.Y, cell.X] = '.';
                        }
                    }
                }
                else
                {
                    if (map[cell.Y, cell.X] == ' ')
                    {
                        map[cell.Y, cell.X] = '.';
                    }
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
            if (door.X > 0 && door.Y > 0)
            {
                if (door.Y >= 0 && door.Y < GenerationConstants.MapHeight && 
                    door.X >= 0 && door.X < GenerationConstants.MapWidth)
                {
                    map[door.Y, door.X] = '+';
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

        for (int y = 0; y < winHeight; y++)
        {
            for (int x = 0; x < winWidth; x++)
            {
                char c = map[y, x];

                if (c == '\0') c = ' ';
                uint color = GetColorForChar(c);
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
        foreach(var enemy in viewModel.Enemies)
        {
                    string[] enemyInfo =
        {
            $"Enemy: {enemy.EnemyType}",
            $"Agil: {enemy.Agil}",
            $"Str: {enemy.Str}",
            $"HP: {enemy.ActHP}/{enemy.HP}",
        };
        y++;
        int partsWithEn = winWidth / enemyInfo.Length;
        for (int i = 0; i < enemyInfo.Length; i++)
            {
                string stat = enemyInfo[i];
                int x = partsWithEn * i + (partsWithEn - stat.Length) / 2 + startX;
                if (x < startX) x = startX;
                if (x + stat.Length >= startX + winWidth) continue;
                NCursesMethods.Print(stat, y, x);
            }
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

