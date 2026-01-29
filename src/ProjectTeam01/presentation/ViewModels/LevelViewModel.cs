using System.Collections.Generic;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.presentation.ViewModels;

/// Представление уровня для отображения на фронтенде
public class LevelViewModel
{
    public List<RoomViewModel> Rooms { get; set; } = new();
    public List<CorridorViewModel> Corridors { get; set; } = new();
    public Position ExitPosition { get; set; }
    public Position StartPosition { get; set; }
    public int LevelNumber { get; set; }
}

