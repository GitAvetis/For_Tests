using ProjectTeam01.domain.Characters;

namespace ProjectTeam01.domain.Combat;

/// Сервис для расчета урона врагов
internal static class EnemyDamageCalculator
{
    public static int CalculateDamage(Enemy enemy, Hero? targetHero = null)
    {
        return enemy switch
        {
            Vampire => CalculateVampireDamage(targetHero),
            Ogre => CalculateOgreDamage(enemy),
            _ => CalculateStandardDamage(enemy)
        };
    }

    private static int CalculateStandardDamage(Enemy enemy)
    {
        return 5 + enemy.BaseStrength * 5;
    }

    private static int CalculateVampireDamage(Hero? hero)
    {
        if (hero == null)
            return 0;
        return hero.MaxHp / BattleConstants.MaxHpPart;
    }

    private static int CalculateOgreDamage(Enemy ogre)
    {
        if (ogre is not Ogre ogreEnemy)
            return 0;

        int damage = 0;
        if (!ogreEnemy.OgreCooldown)
        {
            damage = ogre.BaseStrength * 8;
            ogreEnemy.OgreCooldown = true;
        }
        else
        {
            ogreEnemy.OgreCooldown = false;
        }
        return damage;
    }
}
