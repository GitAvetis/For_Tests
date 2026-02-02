using ProjectTeam01.domain.Items;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Effects;

namespace ProjectTeam01.presentation.ViewModels;

/// Представление предмета для отображения на фронтенде
public class ItemViewModel
{
    public Position Position { get; set; }
    public ItemType Type { get; set; }   
    // Специфичные свойства для разных типов предметов
    public WeaponTypeEnum? WeaponType { get; set; }
    public int? StrengthBonus { get; set; }
    
    public int? HealthValue { get; set; } // Для Food
    
    public EffectTypeEnum? ElixirType { get; set; } // Для Elixir
    
    public ScrollTypeEnum? ScrollType { get; set; } // Для Scroll
    
    public int? Price { get; set; } // Для Treasure
}

