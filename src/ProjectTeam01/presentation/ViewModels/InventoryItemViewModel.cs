using ProjectTeam01.domain.Effects;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.presentation.ViewModels;

/// Представление предмета из инвентаря игрока для отображения на фронтенде
/// Отличается от ItemViewModel тем, что не содержит Position (предметы в инвентаре не имеют позиции на карте)
public class InventoryItemViewModel
{
    public ItemType Type { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    // Специфичные свойства для разных типов предметов
    public WeaponTypeEnum? WeaponType { get; set; }
    public int? StrengthBonus { get; set; }

    public int? HealthValue { get; set; } // Для Food

    public EffectTypeEnum? ElixirType { get; set; } // Для Elixir

    public ScrollTypeEnum? ScrollType { get; set; } // Для Scroll

    public int? Price { get; set; } // Для Treasure
}

