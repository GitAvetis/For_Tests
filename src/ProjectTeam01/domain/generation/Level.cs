using System;
using System.Collections.Generic;
using System.Linq;
using ProjectTeam01.domain;
using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.domain.generation;

/// Представляет уровень подземелья с комнатами, коридорами и сущностями
public class Level
{
    /// Геометрия уровня
    public List<Room> Rooms { get; set; }
    public List<Corridor> Corridors { get; set; }
    public int LevelNumber { get; set; }
    public Position ExitPosition { get; set; }
    public Position StartPosition { get; set; }
    
    /// Сущности уровня 
    private readonly List<IGameObject> _entities;

    /// Индекс для быстрого поиска сущностей по координатам
    private Dictionary<Position, List<IGameObject>> _positionIndex;
    private bool _indexDirty = true;

    /// Конструктор уровня
    public Level()
    {
        Rooms = new List<Room>();
        Corridors = new List<Corridor>();
        _entities = new List<IGameObject>();
        _positionIndex = new Dictionary<Position, List<IGameObject>>();
        ExitPosition = new Position();
        StartPosition = new Position();
    }
    
    /// Получить все сущности уровня
    public IReadOnlyList<IGameObject> Entities => _entities.AsReadOnly();
    
    /// Проверить, можно ли пройти по позиции (геометрия)
    public bool IsWalkable(int x, int y)
    {
        var pos = new Position(x, y);
        
        // Проверка комнат
        foreach (var room in Rooms)
        {
            if (pos.X >= room.TopLeft.X && pos.X <= room.BottomRight.X &&
                pos.Y >= room.TopLeft.Y && pos.Y <= room.BottomRight.Y)
                return true;
        }
        
        // Проверка коридоров
        foreach (var corridor in Corridors)
        {
            if (corridor.Cells != null && corridor.Cells.Contains(pos))
                return true;
        }
        
        return false;
    }
    
    /// Проверить, заблокирована ли клетка актором (игрок/враг).
    /// Предметы клетку не блокируют.
    public bool IsOccupied(int x, int y)
    {
        EnsureIndex();
        var pos = new Position(x, y);
        return _positionIndex.TryGetValue(pos, out var list) && list.Any(IsBlockingEntity);
    }

    /// Проверить, есть ли на клетке хоть какая-то сущность (включая предметы)
    public bool HasAnyEntityAt(int x, int y)
    {
        EnsureIndex();
        return _positionIndex.ContainsKey(new Position(x, y));
    }
    
    /// Получить сущность по позиции
    public IGameObject? GetEntityAt(int x, int y)
    {
        EnsureIndex();
        if (_positionIndex.TryGetValue(new Position(x, y), out var list) && list.Count > 0)
            return list[0];

        return null;
    }
    
    /// Получить все сущности на позиции
    public IReadOnlyList<IGameObject> GetEntitiesAt(int x, int y)
    {
        EnsureIndex();
        var pos = new Position(x, y);
        return _positionIndex.TryGetValue(pos, out var list)
            ? list.AsReadOnly()
            : Array.Empty<IGameObject>();
    }
    
    /// Получить всех врагов на уровне
    internal IReadOnlyList<Characters.Enemy> GetEnemies()
    {
        return _entities.OfType<Characters.Enemy>().ToList().AsReadOnly();
    }
    
    /// Получить все предметы на уровне
    internal IReadOnlyList<Items.Item> GetItems()
    {
        return _entities.OfType<Items.Item>().ToList().AsReadOnly();
    }
    
    /// Найти комнату, в которой находится указанная позиция
    internal Room? FindRoomAt(int x, int y)
    {
        var pos = new Position(x, y);
        foreach (var room in Rooms)
        {
            if (pos.X >= room.TopLeft.X && pos.X <= room.BottomRight.X &&
                pos.Y >= room.TopLeft.Y && pos.Y <= room.BottomRight.Y)
                return room;
        }
        return null;
    }
    
    /// Найти коридор, в котором находится указанная позиция
    internal Corridor? FindCorridorAt(int x, int y)
    {
        var pos = new Position(x, y);
        foreach (var corridor in Corridors)
        {
            if (corridor.Cells != null && corridor.Cells.Contains(pos))
                return corridor;
        }
        return null;
    }
    
    /// Добавить сущность на уровень
    public void AddEntity(IGameObject entity)
    {
        if (entity == null) return;
        
        _entities.Add(entity);
        _indexDirty = true;
    }
    
    /// Удалить сущность с уровня
    public void RemoveEntity(IGameObject entity)
    {
        if (entity == null) return;
        
        _entities.Remove(entity);
        _indexDirty = true;
    }
    
    /// Обновить позицию сущности в индексе (при движении)
    public void UpdateEntityPosition(IGameObject entity, Position oldPosition)
    {
        EnsureIndex();

        if (_positionIndex.TryGetValue(oldPosition, out var oldList))
        {
            oldList.Remove(entity);
            if (oldList.Count == 0)
                _positionIndex.Remove(oldPosition);
        }

        if (!_positionIndex.TryGetValue(entity.Position, out var newList))
        {
            newList = new List<IGameObject>();
            _positionIndex[entity.Position] = newList;
        }
        newList.Add(entity);
    }
    
    /// Обновить индекс позиций (ленивая инициализация)
    private void EnsureIndex()
    {
        if (_indexDirty)
        {
            _positionIndex = new Dictionary<Position, List<IGameObject>>();

            foreach (var entity in _entities)
            {
                if (!_positionIndex.TryGetValue(entity.Position, out var list))
                {
                    list = new List<IGameObject>();
                    _positionIndex[entity.Position] = list;
                }
                list.Add(entity);
            }
            
            _indexDirty = false;
        }
    }

    private static bool IsBlockingEntity(IGameObject entity)
    {
        return entity is Hero || entity is Enemy;
    }
}

