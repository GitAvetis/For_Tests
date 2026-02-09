using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Session;
// отвечает за логику перемещения
internal partial class GameSession
{
    /// Переместить сущность на новую позицию
    public bool MoveEntity(IGameObject entity, int newX, int newY)
    {
        if (entity == null) return false;

        if (!_currentLevel.IsWalkable(newX, newY))
            return false;

        if (entity.Position.X != newX || entity.Position.Y != newY)
        {
            if (_currentLevel.IsOccupied(newX, newY))
                return false;
        }

        var oldPos = entity.Position;
        entity.MoveTo(newX, newY);
        _currentLevel.UpdateEntityPosition(entity, oldPos);

        return true;
    }

    /// Переместить игрока
    public bool MovePlayer(int newX, int newY)
    {
        var oldRoom = _currentLevel.FindRoomAt(Player.Position.X, Player.Position.Y);
        
        bool moved = MoveEntity(Player, newX, newY);
        if (moved)
        {
            _statistics.RecordMove(1);
            CheckForInteractions(Player.Position);
            
            UpdateFogOfWar(oldRoom);
        }
        return moved;
    }
    
    /// Обновить туман войны при перемещении игрока
    private void UpdateFogOfWar(Room? oldRoom)
    {
        var newRoom = _currentLevel.FindRoomAt(Player.Position.X, Player.Position.Y);
        var newCorridor = _currentLevel.FindCorridorAt(Player.Position.X, Player.Position.Y);
        
        bool isInDoor = IsPlayerInDoor(newRoom);
        
        int? corridorIndex = null;
        Corridor? doorCorridor = null;
        
        if (newCorridor != null)
        {
            corridorIndex = FindCorridorIndex(newCorridor);
            doorCorridor = newCorridor;
        }
        else if (isInDoor && newRoom != null)
        {
            doorCorridor = FindCorridorForDoor(newRoom, Player.Position);
            if (doorCorridor != null)
            {
                corridorIndex = FindCorridorIndex(doorCorridor);
            }
        }
        
        // Обработка комнат
        if (oldRoom != null && newRoom == null)
        {
            _fogOfWar.ExitRoom();
        }
        else if (oldRoom == null && newRoom != null)
        {
            _fogOfWar.ExitCorridor();
            _fogOfWar.EnterRoom(newRoom, Player.Position);
        }
        else if (oldRoom != null && newRoom != null && oldRoom.Sector != newRoom.Sector)
        {
            _fogOfWar.ExitRoom();
            _fogOfWar.ExitCorridor();
            _fogOfWar.EnterRoom(newRoom, Player.Position);
        }
        else if (oldRoom != null && newRoom != null && oldRoom.Sector == newRoom.Sector)
        {
            _fogOfWar.ExitCorridor();
            _fogOfWar.EnterRoom(newRoom, Player.Position);
        }
        
        // Обработка коридоров (включая двери)
        if (newRoom == null && newCorridor != null && corridorIndex.HasValue)
        {
            _fogOfWar.ExitRoom();
            _fogOfWar.EnterCorridor(newCorridor, Player.Position, corridorIndex.Value);
        }
        else if (isInDoor && doorCorridor != null && corridorIndex.HasValue)
        {
            _fogOfWar.EnterCorridor(doorCorridor, Player.Position, corridorIndex.Value);
        }
        else if (newRoom == null && newCorridor == null && !isInDoor)
        {
            _fogOfWar.ExitCorridor();
        }
        else if (newCorridor != null && corridorIndex.HasValue)
        {
            _fogOfWar.EnterCorridor(newCorridor, Player.Position, corridorIndex.Value);
        }
    }
    
    /// Проверить, находится ли игрок в двери
    private bool IsPlayerInDoor(Room? room)
    {
        if (room == null) return false;
        
        var playerPos = Player.Position;
        if (room.Doors == null) return false;
        
        return room.Doors.Any(door => 
            (door.X != 0 || door.Y != 0) && door.X == playerPos.X && door.Y == playerPos.Y);
    }
    
    /// Найти коридор, который использует указанную дверь
    private Corridor? FindCorridorForDoor(Room room, Position doorPosition)
    {
        foreach (var corridor in _currentLevel.Corridors)
        {
            if (corridor.Points != null && corridor.Points.Any(p => p.X == doorPosition.X && p.Y == doorPosition.Y))
            {
                return corridor;
            }
        }
        
        return null;
    }
    

    /// Проверить взаимодействия на позиции игрока
    private void CheckForInteractions(Position position)
    {
        var entitiesAtPosition = _currentLevel.GetEntitiesAt(position.X, position.Y)
                                             .Where(e => e != Player)
                                             .ToList();

        foreach (var entity in entitiesAtPosition)
        {
            if (entity is Items.Item item)
            {
                PickupItem(item);
            }
        }
    }
}


