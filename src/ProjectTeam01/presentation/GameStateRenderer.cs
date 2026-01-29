using System;
using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Items;
using ProjectTeam01.domain.generation;
using ProjectTeam01.presentation.ViewModels;

namespace ProjectTeam01.presentation;
///СУЩЕСТВУЕТ СЕЙЧАС ДЛЯ СТАТИЧНОЙ ОТРИСОВКИ УРОВНЯ (УДАЛЯЕМ КОГДА ПОЯВИТСЯ РЕАЛЬНЫЙ ФРОНТ)
/// Рендерер для отрисовки состояния игры в консоль
/// Работает с ViewModels (новый подход)
internal static class GameStateRenderer
{
    /// Отрисовывает состояние игры в консоль
    public static void Render(GameStateViewModel viewModel)
    {
        // Создаем карту
        char[,] map = new char[GenerationConstants.MapHeight, GenerationConstants.MapWidth];
        
        // Инициализируем карту пробелами
        for (int y = 0; y < GenerationConstants.MapHeight; y++)
        {
            for (int x = 0; x < GenerationConstants.MapWidth; x++)
            {
                map[y, x] = ' ';
            }
        }
        
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
        PrintMap(map);
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
            char enemySymbol = GetEnemySymbol(enemy.EnemyType);
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
    
    /// Выводит карту в консоль
    private static void PrintMap(char[,] map)
    {
        Console.Clear();
        Console.WriteLine("=== DUNGEON LEVEL ===");
        Console.WriteLine();
        
        for (int y = 0; y < GenerationConstants.MapHeight; y++)
        {
            for (int x = 0; x < GenerationConstants.MapWidth; x++)
            {
                Console.Write(map[y, x]);
            }
            Console.WriteLine();
        }
        
        Console.WriteLine();
        Console.WriteLine("Legend:");
        Console.WriteLine("  # = Wall");
        Console.WriteLine("  . = Floor");
        Console.WriteLine("  + = Door");
        Console.WriteLine("  S = Start");
        Console.WriteLine("  E = Exit");
        Console.WriteLine();
        Console.WriteLine("Entities:");
        Console.WriteLine("  @ = Player");
        Console.WriteLine("  z = Zombie");
        Console.WriteLine("  v = Vampire");
        Console.WriteLine("  g = Ghost");
        Console.WriteLine("  O = Ogre");
        Console.WriteLine("  s = Snake");
        Console.WriteLine("  m = Mimic");
        Console.WriteLine("  $ = Treasure");
        Console.WriteLine("  F = Food");
        Console.WriteLine("  e = Elixir");
        Console.WriteLine("  ? = Scroll");
        Console.WriteLine("  W = Weapon");
    }
}

