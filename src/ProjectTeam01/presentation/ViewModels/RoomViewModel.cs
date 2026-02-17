using System;
using System.Collections.Generic;
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
    
    // Данные для тумана войны
    public int Sector { get; set; }
    public bool IsVisited { get; set; } 
    public bool IsCurrentlyInRoom { get; set; } 
    public Dictionary<Position, bool> VisibilityMap { get; set; } = new();
}

