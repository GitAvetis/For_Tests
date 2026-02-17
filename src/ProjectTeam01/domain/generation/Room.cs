namespace ProjectTeam01.domain.generation;

/// Направления для соединений комнат
public enum Direction
{
    Top = 0,
    Right = 1,
    Bottom = 2,
    Left = 3
}

/// Представляет комнату на уровне
public class Room
{
    public int Sector { get; set; } // Индекс секции (0-8)

    // Геометрия комнаты
    public Position TopLeft { get; set; }
    public Position BottomRight { get; set; }

    // Двери в комнате (по направлениям)
    public Position[] Doors { get; set; }

    // Соединения с другими комнатами
    public Room[] Connections { get; set; }

    // Флаги специальных комнат
    public bool IsStartRoom { get; set; }
    public bool IsEndRoom { get; set; }

    public Room()
    {
        Doors = new Position[4];
        Connections = new Room[4];
        TopLeft = new Position();
        BottomRight = new Position();
        Sector = -1; // -1 означает неинициализированную комнату
    }

    /// Получить ширину комнаты
    public int Width => BottomRight.X - TopLeft.X + 1;

    /// Получить высоту комнаты
    public int Height => BottomRight.Y - TopLeft.Y + 1;
}
