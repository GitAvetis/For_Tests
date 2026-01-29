using System;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.presentation.ViewModels;

/// Представление комнаты для отображения на фронтенде
public class RoomViewModel
{
    public Position TopLeft { get; set; }
    public Position BottomRight { get; set; }
    public Position[] Doors { get; set; } = Array.Empty<Position>();
    public bool IsStartRoom { get; set; }
    public bool IsEndRoom { get; set; }
}

