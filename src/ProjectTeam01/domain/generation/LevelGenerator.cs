using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProjectTeam01.unit_tests")]

namespace ProjectTeam01.domain.generation;

/// Константы для генерации уровней
public static class GenerationConstants
{
    public const int RoomsPerSide = 3;
    public const int MaxRoomsNumber = RoomsPerSide * RoomsPerSide;
    public const int MaxCorridorsNumber = 12;

    public const int MapHeight = 30;
    public const int MapWidth = 90;

    // Размеры регионов (каждая секция 3x3)
    public const int SectorHeight = MapHeight / RoomsPerSide;
    public const int SectorWidth = MapWidth / RoomsPerSide;

    // Размеры комнат
    public const int MinRoomWidth = 6;
    public const int MaxRoomWidth = SectorWidth - 2;  
    public const int MinRoomHeight = 5;
    public const int MaxRoomHeight = SectorHeight - 2;
}
/// Генератор уровней подземелья
public class LevelGenerator
{
    private Random _random;
    public LevelGenerator()
    {
        _random = new Random();
    }

    /// Генерирует уровень с комнатами и коридорами
    public Level GenerateLevel()
    {
        var level = new Level();
        var rooms = new Room[GenerationConstants.RoomsPerSide + 2, GenerationConstants.RoomsPerSide + 2];
        
        InitializeRooms(rooms);
        GenerateSectors(rooms, level);
        GenerateConnections(rooms, level);
        
        GenerateRoomsGeometry(rooms, level);
        GenerateCorridorsGeometry(rooms, level);
        
        SelectStartAndEndRooms(level);

        return level;
    }

    /// Инициализирует массив комнат
    private void InitializeRooms(Room[,] rooms)
    {
        for (int i = 0; i < GenerationConstants.RoomsPerSide + 2; i++)
        {
            for (int j = 0; j < GenerationConstants.RoomsPerSide + 2; j++)
            {
                rooms[i, j] = new Room
                {
                    Sector = -1
                };
            }
        }
    }

    /// Генерирует секции (комнаты в сетке 3x3)
    private void GenerateSectors(Room[,] rooms, Level level)
    {
        int sector = 0;
        for (int i = 1; i < GenerationConstants.RoomsPerSide + 1; i++)
        {
            for (int j = 1; j < GenerationConstants.RoomsPerSide + 1; j++, sector++)
            {
                rooms[i, j].Sector = sector;
                level.Rooms.Add(rooms[i, j]);
            }
        }
    }

    /// Генерирует геометрию комнат (размеры и позиции)
    private void GenerateRoomsGeometry(Room[,] rooms, Level level)
    {
        for (int i = 0; i < level.Rooms.Count; i++)
        {
            var room = level.Rooms[i];
            
            int width = _random.Next(GenerationConstants.MinRoomWidth, GenerationConstants.MaxRoomWidth + 1);
            int height = _random.Next(GenerationConstants.MinRoomHeight, GenerationConstants.MaxRoomHeight + 1);
            
            int column = room.Sector % GenerationConstants.RoomsPerSide;  
            int row = room.Sector / GenerationConstants.RoomsPerSide;
            
            int leftRangeCoord = column * GenerationConstants.SectorWidth + 1;
            int rightRangeCoord = (column + 1) * GenerationConstants.SectorWidth - width - 1;
            int xCoord = _random.Next(leftRangeCoord, rightRangeCoord + 1);
            
            int upRangeCoord = row * GenerationConstants.SectorHeight + 1;
            int bottomRangeCoord = (row + 1) * GenerationConstants.SectorHeight - height - 1;
            int yCoord = _random.Next(upRangeCoord, bottomRangeCoord + 1);
            
            room.TopLeft = new Position(xCoord, yCoord);
            room.BottomRight = new Position(xCoord + width - 1, yCoord + height - 1);
            
            GenerateDoors(room);
        }
    }

    /// Генерирует двери в комнате
    private void GenerateDoors(Room room)
    {
        if (room.Connections[(int)Direction.Top] != null)
        {
            int doorX = _random.Next(room.TopLeft.X + 1, room.BottomRight.X);
            room.Doors[(int)Direction.Top] = new Position(doorX, room.TopLeft.Y);
        }

        if (room.Connections[(int)Direction.Right] != null)
        {
            int doorY = _random.Next(room.TopLeft.Y + 1, room.BottomRight.Y);
            room.Doors[(int)Direction.Right] = new Position(room.BottomRight.X, doorY);
        }

        if (room.Connections[(int)Direction.Bottom] != null)
        {
            int doorX = _random.Next(room.TopLeft.X + 1, room.BottomRight.X);
            room.Doors[(int)Direction.Bottom] = new Position(doorX, room.BottomRight.Y);
        }

        if (room.Connections[(int)Direction.Left] != null)
        {
            int doorY = _random.Next(room.TopLeft.Y + 1, room.BottomRight.Y);
            room.Doors[(int)Direction.Left] = new Position(room.TopLeft.X, doorY);
        }
    }

    /// Структура для представления ребра между двумя комнатами
    internal struct Edge
    {
        public int Room1Index; 
        public int Room2Index;
        public Direction Direction1;
        public Direction Direction2;
    }

    /// Генерирует соединения между комнатами используя DSU
    internal void GenerateConnections(Room[,] rooms, Level level)
    {
        var edges = GenerateEdgesForRooms(level);
        
        ShuffleEdges(edges);
        
        var dsu = new DSU(level.Rooms.Count);
        
        foreach (var edge in edges)
        {
            if (dsu.UnionSets(edge.Room1Index, edge.Room2Index))
            {
                var room1 = level.Rooms[edge.Room1Index];
                var room2 = level.Rooms[edge.Room2Index];
                
                room1.Connections[(int)edge.Direction1] = room2;
                room2.Connections[(int)edge.Direction2] = room1;
            }
        }

        if (!dsu.IsConnected())
        {
            throw new InvalidOperationException(
                $"Граф комнат не связный! Найдено {dsu.GetConnectedComponentsCount()} компонент связности из {level.Rooms.Count} комнат.");
        }
    }

    /// Генерирует все возможные ребра между соседними комнатами в сетке
    internal List<Edge> GenerateEdgesForRooms(Level level)
    {
        var edges = new List<Edge>();
        
        // Генерируем горизонтальные ребра
        for (int i = 1; i < GenerationConstants.RoomsPerSide + 1; i++)
        {
            for (int j = 1; j < GenerationConstants.RoomsPerSide; j++)
            {
                int sector1 = (i - 1) * GenerationConstants.RoomsPerSide + (j - 1);
                int sector2 = (i - 1) * GenerationConstants.RoomsPerSide + j;
                
                edges.Add(new Edge
                {
                    Room1Index = sector1,
                    Room2Index = sector2,
                    Direction1 = Direction.Right,
                    Direction2 = Direction.Left
                });
            }
        }
        
        // Генерируем вертикальные ребра
        for (int i = 1; i < GenerationConstants.RoomsPerSide; i++)
        {
            for (int j = 1; j < GenerationConstants.RoomsPerSide + 1; j++)
            {
                int sector1 = (i - 1) * GenerationConstants.RoomsPerSide + (j - 1);
                int sector2 = i * GenerationConstants.RoomsPerSide + (j - 1);
                
                edges.Add(new Edge
                {
                    Room1Index = sector1,
                    Room2Index = sector2,
                    Direction1 = Direction.Bottom,
                    Direction2 = Direction.Top
                });
            }
        }
        
        return edges;
    }

    /// Перемешивает массив ребер случайным образом
    internal void ShuffleEdges(List<Edge> edges)
    {
        for (int i = edges.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (edges[i], edges[j]) = (edges[j], edges[i]);
        }
    }
    /// Генерирует геометрию коридоров
    internal void GenerateCorridorsGeometry(Room[,] rooms, Level level)
    {
        for (int i = 1; i < GenerationConstants.RoomsPerSide + 1; i++)
        {
            for (int j = 1; j < GenerationConstants.RoomsPerSide + 1; j++)
            {
                var curRoom = rooms[i, j];
                
                // Проверяем соединение справа 
                if (curRoom.Connections[(int)Direction.Right] != null)
                {
                    var rightRoom = curRoom.Connections[(int)Direction.Right];
                    // Проверяем, что соединение двунаправленное
                    if (rightRoom.Connections[(int)Direction.Left] == curRoom)
                    {
                        var corridor = GenerateHorizontalCorridor(curRoom, rightRoom, rooms);
                        level.Corridors.Add(corridor);
                    }
                }
                
                // Проверяем соединение снизу
                if (curRoom.Connections[(int)Direction.Bottom] != null)
                {
                    var bottomRoom = curRoom.Connections[(int)Direction.Bottom];
                    var corridor = GenerateVerticalCorridor(curRoom, bottomRoom, rooms);
                    level.Corridors.Add(corridor);
                }
            }
        }
    }

    /// Генерирует горизонтальный коридор между двумя комнатами
    internal Corridor GenerateHorizontalCorridor(Room leftRoom, Room rightRoom, Room[,] rooms)
    {
        var corridor = new Corridor
        {
            Type = CorridorType.LeftToRight
        };
        
        var leftDoor = leftRoom.Doors[(int)Direction.Right];
        var rightDoor = rightRoom.Doors[(int)Direction.Left];
        
        if (leftDoor.Y == rightDoor.Y)
        {
            corridor.Points.Add(leftDoor);
            corridor.Points.Add(rightDoor);
        }
        else
        {
            int bendX = _random.Next(Math.Min(leftDoor.X, rightDoor.X) + 1, Math.Max(leftDoor.X, rightDoor.X));
            
            corridor.Points.Add(leftDoor);
            corridor.Points.Add(new Position(bendX, leftDoor.Y));
            corridor.Points.Add(new Position(bendX, rightDoor.Y));
            corridor.Points.Add(rightDoor);
        }
        
        GenerateCorridorCells(corridor);
        
        return corridor;
    }

    /// Генерирует вертикальный коридор между двумя комнатами
    internal Corridor GenerateVerticalCorridor(Room topRoom, Room bottomRoom, Room[,] rooms)
    {
        var corridor = new Corridor
        {
            Type = CorridorType.TopToBottom
        };
        
        var topDoor = topRoom.Doors[(int)Direction.Bottom];
        var bottomDoor = bottomRoom.Doors[(int)Direction.Top];
        
        if (topDoor.X == bottomDoor.X)
        {
            corridor.Points.Add(topDoor);
            corridor.Points.Add(bottomDoor);
        }
        else
        {
            int bendY = _random.Next(Math.Min(topDoor.Y, bottomDoor.Y) + 1, Math.Max(topDoor.Y, bottomDoor.Y));
            
            corridor.Points.Add(topDoor);
            corridor.Points.Add(new Position(topDoor.X, bendY));
            corridor.Points.Add(new Position(bottomDoor.X, bendY));
            corridor.Points.Add(bottomDoor);
        }
        
        GenerateCorridorCells(corridor);
        
        return corridor;
    }

    /// Генерирует все клетки коридора на основе ключевых точек
    private void GenerateCorridorCells(Corridor corridor)
    {
        if (corridor.Points == null || corridor.Points.Count < 2)
            return;
        
        corridor.Cells.Clear();
        
        for (int i = 0; i < corridor.Points.Count - 1; i++)
        {
            var start = corridor.Points[i];
            var end = corridor.Points[i + 1];
            
            bool includeEnd = (i == corridor.Points.Count - 2);
            AddCellsBetweenPoints(corridor.Cells, start, end, includeEnd);
        }
    }
    
    /// Добавляет все клетки между двумя точками используя алгоритм Брезенхема
    private void AddCellsBetweenPoints(List<Position> cells, Position start, Position end, bool includeEnd = true)
    {
        int x0 = start.X;
        int y0 = start.Y;
        int x1 = end.X;
        int y1 = end.Y;
        
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        
        int x = x0;
        int y = y0;
        
        while (true)
        {
            cells.Add(new Position(x, y));
            
            if (x == x1 && y == y1)
            {
                {
                    cells.RemoveAt(cells.Count - 1);
                }
                break;
            }
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }

    /// Выбирает стартовую и конечную комнату
    internal void SelectStartAndEndRooms(Level level)
    {
        if (level.Rooms.Count == 0)
            return;
        
        int startRoomIndex = _random.Next(0, level.Rooms.Count);
        var startRoom = level.Rooms[startRoomIndex];
        startRoom.IsStartRoom = true;
        
        int startX = _random.Next(startRoom.TopLeft.X + 1, startRoom.BottomRight.X - 1);
        int startY = _random.Next(startRoom.TopLeft.Y + 1, startRoom.BottomRight.Y - 1);
        level.StartPosition = new Position(startX, startY);
        
        int endRoomIndex;
        do
        {
            endRoomIndex = _random.Next(0, level.Rooms.Count);
        }
        while (endRoomIndex == startRoomIndex);
        
        var endRoom = level.Rooms[endRoomIndex];
        endRoom.IsEndRoom = true;
        
        int endX = _random.Next(endRoom.TopLeft.X + 2, endRoom.BottomRight.X - 2);
        int endY = _random.Next(endRoom.TopLeft.Y + 2, endRoom.BottomRight.Y - 2);
        level.ExitPosition = new Position(endX, endY);
	}
}
