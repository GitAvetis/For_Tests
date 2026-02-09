using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Effects;

namespace ProjectTeam01.domain.Combat;

internal static class PlayerDamageCalculator
{
    
    public static int CalculateDamage(Hero hero)
    {
        int totalStrength = hero.BaseStrength + 
            hero.ActiveEffectManager.GetTotalStatBonus(EffectTypeEnum.BuffStrength);
        
        if (hero.WeaponManager.EquippedWeapon != null)
        {
            int weaponStrength = hero.WeaponManager.EquippedWeapon.StrengthBonus;
            return 10 + weaponStrength * 4 + totalStrength * 3;
        }
        else
        {
            return 10 + totalStrength * 3;
        }
    }
}
