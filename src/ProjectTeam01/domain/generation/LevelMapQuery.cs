namespace ProjectTeam01.domain.generation;

/// Адаптер для использования Level через интерфейс IMapQuery для работы с картой без прямой зависимости от Level.

internal sealed class LevelMapQuery : IMapQuery
{
    private readonly Level _level;

    public LevelMapQuery(Level level)
    {
        _level = level ?? throw new ArgumentNullException(nameof(level));
    }

    public bool IsOccupied(int x, int y)
    {
        if (!_level.IsWalkable(x, y))
            return true;

        return _level.IsOccupied(x, y);
    }

    public int GetDistance(int x1, int y1, int x2, int y2)
    {
        if (x1 == x2 && y1 == y2)
            return 0;

        if (!_level.IsWalkable(x1, y1))
            return int.MaxValue;
        if (!_level.IsWalkable(x2, y2))
            return int.MaxValue;

        int height = GenerationConstants.MapHeight;
        int width = GenerationConstants.MapWidth;

        if (x1 < 0 || x1 >= width || y1 < 0 || y1 >= height)
            return int.MaxValue;
        if (x2 < 0 || x2 >= width || y2 < 0 || y2 >= height)
            return int.MaxValue;

        var visited = new bool[height, width];
        var q = new Queue<(int x, int y, int d)>();

        visited[y1, x1] = true;
        q.Enqueue((x1, y1, 0));

        while (q.Count > 0)
        {
            var (cx, cy, cd) = q.Dequeue();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = cx + dx;
                    int ny = cy + dy;

                    if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        continue;
                    if (visited[ny, nx])
                        continue;

                    if (!_level.IsWalkable(nx, ny))
                        continue;

                    if (_level.IsOccupied(nx, ny) && !(nx == x2 && ny == y2))
                        continue;

                    if (nx == x2 && ny == y2)
                        return cd + 1;

                    visited[ny, nx] = true;
                    q.Enqueue((nx, ny, cd + 1));
                }
            }
        }

        return int.MaxValue;
    }

    public int GetMapLevel()
    {
        return _level.LevelNumber;
    }

    public void RemoveObject(IGameObject obj)
    {
        _level.RemoveEntity(obj);
    }

    public Room? FindRoomAt(int x, int y)
    {
        return _level.FindRoomAt(x, y);
    }

    public Corridor? FindCorridorAt(int x, int y)
    {
        return _level.FindCorridorAt(x, y);
    }
}


