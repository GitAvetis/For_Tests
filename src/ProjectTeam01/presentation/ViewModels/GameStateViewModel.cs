using System.Collections.Generic;

namespace ProjectTeam01.presentation.ViewModels;

/// Представление состояния игры для отображения на фронтенде
public class GameStateViewModel
{
    public PlayerViewModel Player { get; set; } = null!;
    public List<EnemyViewModel> Enemies { get; set; } = new();
    public List<ItemViewModel> Items { get; set; } = new(); // Предметы на карте (с Position)
    public LevelViewModel Level { get; set; } = null!;
    public int CurrentLevelNumber { get; set; }
    
    // Инвентарь игрока (предметы без Position)
    public List<InventoryItemViewModel> InventoryWeapons { get; set; } = new();
    public List<InventoryItemViewModel> InventoryFood { get; set; } = new();
    public List<InventoryItemViewModel> InventoryElixirs { get; set; } = new();
    public List<InventoryItemViewModel> InventoryScrolls { get; set; } = new();
}

