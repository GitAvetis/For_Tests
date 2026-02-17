namespace ProjectTeam01.domain.Combat;

// Отвечает за логику боя, вероятность удара в зависимости от ловкости
internal class BattleService
{
    /// Проверить, попал ли атакующий по цели
    public static bool HitSuccess(int attackerBaseAgility, int targetBaseAgility)
    {
        double hitChance = BattleConstants.InitialHitChance 
            + (attackerBaseAgility - targetBaseAgility - BattleConstants.StandardAgility) 
            * BattleConstants.AgilityFactor;

        // Ограничиваем шанс пределами (0-100%)
        int hitChanceInt = (int)Math.Round(hitChance);
        if (hitChanceInt < 0) hitChanceInt = 0;
        if (hitChanceInt > 100) hitChanceInt = 100;

        return Random.Shared.Next(0, 100) < hitChanceInt;
    }
}

