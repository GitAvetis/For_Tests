namespace ProjectTeam01.datalayer.Models;

/// Модель сохранения для Level (геометрия уровня)
internal class LevelSave
{
    public int LevelNumber { get; set; }
    public List<RoomSave> Rooms { get; set; } = new();
    public List<CorridorSave> Corridors { get; set; } = new();
    public PositionSave StartPosition { get; set; } = new();
    public PositionSave ExitPosition { get; set; } = new();
}

