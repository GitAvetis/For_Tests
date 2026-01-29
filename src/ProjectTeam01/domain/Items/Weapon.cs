namespace ProjectTeam01.domain.Items
{
    public enum WeaponTypeEnum
    {
        Axe,
        Dagger,
        Sword,
        Bow
    }
    internal class Weapon : Item
    {
        public Weapon(WeaponTypeEnum weaponType, int posX, int posY) : base(ItemType.Weapon, posX, posY)
        {
            WeaponType = weaponType;
        }

        public WeaponTypeEnum WeaponType { get; }

        public int StrengthBonus
        {
            get
            {
                switch (WeaponType)
                {
                    case WeaponTypeEnum.Axe:
                        return 5;
                    case WeaponTypeEnum.Dagger:
                        return 2;
                    case WeaponTypeEnum.Sword:
                        return 3;
                    case WeaponTypeEnum.Bow:
                        return 4;
                    default:
                        return 0;
                }
            }
        }
    }
}
