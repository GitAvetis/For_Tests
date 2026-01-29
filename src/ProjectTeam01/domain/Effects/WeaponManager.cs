using ProjectTeam01.domain.Items;

namespace ProjectTeam01.domain.Effects
{
    internal class WeaponManager
    {
        public Weapon? EquippedWeapon { get; private set; }

        public Weapon? EquipWeapon(Weapon weapon)
        {
            Weapon? previousWeapon = EquippedWeapon;
            UnequipWeapon();
            EquippedWeapon = weapon;
            return previousWeapon;
        }

        public Weapon? UnequipWeapon()
        {
            Weapon? previousWeapon = EquippedWeapon;
            if (EquippedWeapon != null)
            {
                EquippedWeapon = null;
            }
            return previousWeapon;
        }

        public int GetStrengthBonus()
        {
            return EquippedWeapon != null ? EquippedWeapon.StrengthBonus : 0;
        }
    }
}
