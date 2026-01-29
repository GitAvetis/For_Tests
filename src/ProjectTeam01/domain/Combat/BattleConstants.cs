namespace ProjectTeam01.domain.Combat;
/// Константы для боевых формул (из rogue_sample)
internal static class BattleConstants
{
    // Шанс попадания
    public const int InitialHitChance = 70;        // Базовый шанс попадания (%)
    public const int StandardAgility = 50;         // Стандартная ловкость
    public const double AgilityFactor = 0.3;       // Множитель ловкости

    // Урон
    public const int InitialDamage = 30;           // Базовый урон
    public const int StandardStrength = 50;        // Стандартная сила
    public const double StrengthFactor = 0.3;      // Множитель силы
    public const int StrengthAddition = 65;        // Добавка к силе при расчете урона с оружием

    // Лут
    public const double LootAgilityFactor = 0.2;   // Множитель ловкости для лута
    public const double LootHpFactor = 0.5;       // Множитель здоровья для лута
    public const double LootStrengthFactor = 0.5;  // Множитель силы для лута
    public const int LootRandomMax = 20;           // Максимальное случайное значение для лута

    // Вампир
    public const int MaxHpPart = 10;               // Делитель для урона вампира (max_hp / 10)
}

