using ProjectTeam01.domain.generation;

namespace ProjectTeam01.datalayer.Models;

/// Модель сохранения для Corridor
internal class CorridorSave
{
    public CorridorType Type { get; set; }
    public List<PositionSave> Points { get; set; } = new();
    public List<PositionSave> Cells { get; set; } = new();
}

