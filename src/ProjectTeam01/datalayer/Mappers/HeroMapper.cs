using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.datalayer.Mappers
{
    internal class HeroMapper
    {
        public static HeroSave ToSave(Hero hero)
        {
            HeroSave heroSave = new HeroSave
            {
                PosX = hero.Position.X,
                PosY = hero.Position.Y,
                CurrentHp = hero.ActualHp,
                BaseStrength = hero.BaseStrength,
                BaseAgility = hero.BaseAgility,
                BaseMaxHp = hero.BaseMaxHp,
                IsHeroSleep = hero.IsHeroSleep
            };

            foreach (var effect in hero.ActiveEffectManager.ActiveEffects)
            {
                heroSave.ActiveEffects.Add(ActiveEffectMapper.ToSave(effect));
            }

            heroSave.EquippedWeapon = hero.WeaponManager.EquippedWeapon != null
                ? ItemMapper.ToSave(hero.WeaponManager.EquippedWeapon)
                : null;

            foreach (var item in hero.HeroBackpack.AllItems)
            {
                heroSave.HeroBackpack.Add(ItemMapper.ToSave(item));
            }
            return heroSave;
        }

        public static Hero FromSave(HeroSave heroSave)
        {
            Hero hero = new Hero(
                heroSave.PosX,
                heroSave.PosY,
                heroSave.BaseMaxHp,
                heroSave.BaseStrength,
                heroSave.BaseAgility,
                heroSave.CurrentHp,
                heroSave.IsHeroSleep
            );

            foreach (var itemSave in heroSave.HeroBackpack)
            {
                var item = ItemMapper.FromSave(itemSave);
                hero.HeroBackpack.Add(item);
            }

            if (heroSave.EquippedWeapon != null)
            {
                if (ItemMapper.FromSave(heroSave.EquippedWeapon) is Weapon weapon)
                {
                    hero.WeaponManager.EquipWeapon(weapon);
                }
            }

            foreach (var effectSave in heroSave.ActiveEffects)
            {
                var effect = ActiveEffectMapper.FromSave(effectSave);
                hero.ActiveEffectManager.AddActiveEffect(effect);
            }



            return hero;
        }
    }
}
