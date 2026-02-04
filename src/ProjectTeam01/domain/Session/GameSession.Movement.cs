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
        bool moved = MoveEntity(Player, newX, newY);
        if (moved)
        {
            _statistics.RecordMove(1);
            CheckForInteractions(Player.Position);
        }
        return moved;
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


