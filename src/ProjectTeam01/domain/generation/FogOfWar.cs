using System.Collections.Generic;
using System.Linq;

namespace ProjectTeam01.domain.generation;

/// Управляет туманом войны - отслеживает видимость комнат и их содержимого
public class FogOfWar
{
    /// Посещенные комнаты (ключ - Sector комнаты)
    private readonly HashSet<int> _visitedRooms;
    
    /// Текущая комната игрока (Sector, null если не в комнате)
    private int? _currentRoomSector;
    
    /// Карта видимости для текущей комнаты (ключ - позиция, значение - видима ли)
    private Dictionary<Position, bool> _currentRoomVisibility;
    
    /// Посещенные сегменты коридоров (ключ - "corridorIndex_segmentIndex", значение - true)
    private readonly HashSet<string> _visitedCorridorSegments;
    
    /// Текущий сегмент коридора игрока (ключ - "corridorIndex_segmentIndex", null если не в коридоре)
    private string? _currentCorridorSegment;

    public FogOfWar()
    {
        _visitedRooms = new HashSet<int>();
        _currentRoomSector = null;
        _currentRoomVisibility = new Dictionary<Position, bool>();
        _visitedCorridorSegments = new HashSet<string>();
        _currentCorridorSegment = null;
    }

    /// Проверить, была ли комната посещена
    public bool IsRoomVisited(int roomSector)
    {
        return _visitedRooms.Contains(roomSector);
    }

    /// Проверить, находится ли игрок в комнате
    public bool IsPlayerInRoom(int roomSector)
    {
        return _currentRoomSector == roomSector;
    }

    /// Получить текущую комнату игрока
    public int? GetCurrentRoomSector()
    {
        return _currentRoomSector;
    }

    /// Проверить, видима ли позиция в текущей комнате
    public bool IsPositionVisible(int x, int y)
    {
        var pos = new Position(x, y);
        
        if (_currentRoomSector == null)
            return false;
        
        if (_currentRoomVisibility.TryGetValue(pos, out bool visible))
            return visible;
        
        return false;
    }

    /// Проверить, должна ли комната показываться на карте (была посещена)
    public bool ShouldShowRoom(int roomSector)
    {
        return _visitedRooms.Contains(roomSector);
    }

    /// Игрок вошел в комнату или переместился в ней
    public void EnterRoom(Room room, Position playerPosition)
    {
        if (room == null) return;
        
        int roomSector = room.Sector;
        
        _visitedRooms.Add(roomSector);
        
        _currentRoomSector = roomSector;
        
        CalculateRoomVisibility(room, playerPosition);
    }

    /// Игрок вышел из комнаты
    public void ExitRoom()
    {
        _currentRoomSector = null;
        _currentRoomVisibility.Clear();
    }
    
    /// Игрок вошел в коридор или переместился в нем
    public void EnterCorridor(Corridor corridor, Position playerPosition, int corridorIndex)
    {
        if (corridor == null) return;
        
        var segmentIndices = GetSegmentIndices(corridor, playerPosition);
        
        foreach (int segmentIndex in segmentIndices)
        {
            string segmentKey = $"{corridorIndex}_{segmentIndex}";
            
            _visitedCorridorSegments.Add(segmentKey);
        }
        
        if (segmentIndices.Count > 0)
        {
            _currentCorridorSegment = $"{corridorIndex}_{segmentIndices[0]}";
        }
    }
    
    /// Определить индексы сегментов коридора для позиции
    /// Точка изгиба может принадлежать двум сегментам
    private List<int> GetSegmentIndices(Corridor corridor, Position position)
    {
        var indices = BresenhamUtils.GetSegmentIndicesForPosition(corridor.Points, position);

        if (indices.Count > 0 && corridor.Points != null && corridor.Points.Count > 2)
        {
            for (int i = 0; i < corridor.Points.Count - 1; i++)
            {
                if (indices.Contains(i) && i < corridor.Points.Count - 2)
                {
                    var end = corridor.Points[i + 1];
                    int distanceToEnd = System.Math.Abs(position.X - end.X) + System.Math.Abs(position.Y - end.Y);
                    if (distanceToEnd <= 1)
                    {
                        if (!indices.Contains(i + 1))
                        {
                            indices.Add(i + 1);
                        }
                    }
                }
            }
        }
        
        return indices;
    }
    
    /// Игрок вышел из коридора
    public void ExitCorridor()
    {
        _currentCorridorSegment = null;
    }
    
    /// Проверить, виден ли сегмент коридора
    public bool IsCorridorSegmentVisible(int corridorIndex, int segmentIndex)
    {
        string segmentKey = $"{corridorIndex}_{segmentIndex}";
        return _visitedCorridorSegments.Contains(segmentKey);
    }
    
    /// Проверить, находится ли игрок в сегменте коридора
    public bool IsPlayerInCorridorSegment(int corridorIndex, int segmentIndex)
    {
        string segmentKey = $"{corridorIndex}_{segmentIndex}";
        return _currentCorridorSegment == segmentKey;
    }

    /// Проверить, находится ли позиция на границе комнаты (на стенах или дверях)
    private bool IsOnRoomBorder(Position pos, Room room)
    {
        bool onTop = pos.Y == room.TopLeft.Y;
        bool onBottom = pos.Y == room.BottomRight.Y;
        bool onLeft = pos.X == room.TopLeft.X;
        bool onRight = pos.X == room.BottomRight.X;
        
        return onTop || onBottom || onLeft || onRight;
    }
    
    /// Определить, вертикальное ли направление тумана (игрок на левой/правой стене)
    /// Логика из rogue_sample: если при движении влево и вправо мы внутри комнаты, значит не вертикальное
    private bool IsVerticalDirectionFog(Position playerPos, Room room)
    {
        var testPosRight = new Position(playerPos.X + 1, playerPos.Y);
        var testPosLeft = new Position(playerPos.X - 1, playerPos.Y);
        
        bool rightInside = testPosRight.X > room.TopLeft.X && testPosRight.X < room.BottomRight.X &&
                           testPosRight.Y > room.TopLeft.Y && testPosRight.Y < room.BottomRight.Y;
        bool leftInside = testPosLeft.X > room.TopLeft.X && testPosLeft.X < room.BottomRight.X &&
                          testPosLeft.Y > room.TopLeft.Y && testPosLeft.Y < room.BottomRight.Y;
        
        return !rightInside && !leftInside;
    }

    /// Вычислить видимость внутри комнаты
    /// Частичный туман появляется только когда игрок на границе комнаты
    private void CalculateRoomVisibility(Room room, Position playerPosition)
    {
        _currentRoomVisibility.Clear();
        
        bool isOnBorder = IsOnRoomBorder(playerPosition, room);
        
        if (!isOnBorder)
        {
            for (int y = room.TopLeft.Y; y <= room.BottomRight.Y; y++)
            {
                for (int x = room.TopLeft.X; x <= room.BottomRight.X; x++)
                {
                    var pos = new Position(x, y);
                    _currentRoomVisibility[pos] = true;
                }
            }
            return;
        }
        
        bool isVertical = IsVerticalDirectionFog(playerPosition, room);
        int playerX = playerPosition.X;
        int playerY = playerPosition.Y;
        
        for (int y = room.TopLeft.Y; y <= room.BottomRight.Y; y++)
        {
            for (int x = room.TopLeft.X; x <= room.BottomRight.X; x++)
            {
                var pos = new Position(x, y);
                
                bool isWall = y == room.TopLeft.Y || y == room.BottomRight.Y ||
                             x == room.TopLeft.X || x == room.BottomRight.X;
                
                if (isWall)
                {
                    _currentRoomVisibility[pos] = true;
                    continue;
                }
                
                int deltaX = x - playerX;
                int deltaY = y - playerY;
                
                bool visible = true;
                
                if (isVertical)
                {
                    if (System.Math.Abs(deltaX) >= System.Math.Abs(deltaY))
                    {
                        visible = false; 
                    }
                }
                else
                {
                    if (System.Math.Abs(deltaX) <= System.Math.Abs(deltaY))
                    {
                        visible = false; 
                    }
                }
                
                _currentRoomVisibility[pos] = visible;
            }
        }
    }

    /// Получить все посещенные комнаты (для сохранения)
    public HashSet<int> GetVisitedRooms()
    {
        return new HashSet<int>(_visitedRooms);
    }

    /// Получить все посещенные сегменты коридоров (для сохранения)
    public HashSet<string> GetVisitedCorridorSegments()
    {
        return new HashSet<string>(_visitedCorridorSegments);
    }

    /// Восстановить состояние из сохранения
    public void RestoreVisitedRooms(HashSet<int> visitedRooms)
    {
        _visitedRooms.Clear();
        foreach (var sector in visitedRooms)
        {
            _visitedRooms.Add(sector);
        }
    }

    /// Восстановить посещенные сегменты коридоров из сохранения
    public void RestoreVisitedCorridorSegments(HashSet<string> visitedCorridorSegments)
    {
        _visitedCorridorSegments.Clear();
        foreach (var segmentKey in visitedCorridorSegments)
        {
            _visitedCorridorSegments.Add(segmentKey);
        }
    }
    
    /// Добавить сегмент коридора как посещенный (для восстановления коридоров между комнатами)
    public void AddVisitedCorridorSegment(string segmentKey)
    {
        _visitedCorridorSegments.Add(segmentKey);
    }
    
    /// Очистить состояние тумана войны (при переходе на новый уровень)
    public void Clear()
    {
        _visitedRooms.Clear();
        _currentRoomSector = null;
        _currentRoomVisibility.Clear();
        _visitedCorridorSegments.Clear();
        _currentCorridorSegment = null;
    }
}
