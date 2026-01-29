using System.Collections.Generic;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.presentation.ViewModels;

/// Представление коридора для отображения на фронтенде
public class CorridorViewModel
{
    public CorridorType Type { get; set; }
    public List<Position> Cells { get; set; } = new();
}

