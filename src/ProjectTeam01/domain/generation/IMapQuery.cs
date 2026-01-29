namespace ProjectTeam01.domain.generation;

/// Интерфейс для запросов к карте/уровню.
internal interface IMapQuery
{
    bool IsOccupied(int x, int y);
    int GetDistance(int x1, int y1, int x2, int y2);
    int GetMapLevel();
    void RemoveObject(IGameObject obj);
    Room? FindRoomAt(int x, int y);
    Corridor? FindCorridorAt(int x, int y);
}

