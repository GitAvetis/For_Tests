using ProjectTeam01.domain.Effects;

namespace ProjectTeam01.domain.Characters
{
    internal class Hero : Character
    {
        private List<ActiveEffect> ActiveEffects { get; set; }
        public ActiveEffectManager ActiveEffectManager { get; }
        public WeaponManager WeaponManager { get; }
        public Backpack HeroBackpack { get; }
        public bool IsHeroSleep { get; protected set; }

        public Hero(int posX, int posY) : base(posX, posY)
        {
            ActiveEffects = new List<ActiveEffect>();
            ActiveEffectManager = new ActiveEffectManager(ActiveEffects, this);
            WeaponManager = new WeaponManager();
            HeroBackpack = new Backpack();
            BaseAgility = 5;
            BaseStrength = 5;
            BaseMaxHp = 150;
            ActualHp = MaxHp;
        }

        //контруктор для воостановления героя из сохранения
        internal Hero(
        int posX,
        int posY,
        int baseMaxHp,
        int baseStrength,
        int baseAgility,
        int currentHp,
        bool isHeroSleep
    ) : base(posX, posY)
        {
            ActiveEffects = new List<ActiveEffect>();
            ActiveEffectManager = new ActiveEffectManager(ActiveEffects, this);
            WeaponManager = new WeaponManager();
            HeroBackpack = new Backpack();

            BaseMaxHp = baseMaxHp;
            BaseStrength = baseStrength;
            BaseAgility = baseAgility;
            ActualHp = currentHp;
            IsHeroSleep = isHeroSleep;

        }
        private int _actualHp;

        public void MaxHpDecrease(int decreaseValue, bool debuffIsAtive)
        {
            if (debuffIsAtive)
            {
                BaseMaxHp -= decreaseValue;
                if (ActualHp > MaxHp)
                {
                    ActualHp = MaxHp;
                }
            }
            else
            {
                BaseMaxHp += decreaseValue;
            }
        }

        public override int ActualHp
        {
            get
            {
                return _actualHp;
            }
            protected set
            {
                if (value >= MaxHp)
                {
                    _actualHp = MaxHp;
                }
                else if (value <= 0)
                {
                    _actualHp = 0;
                }
                else
                {
                    _actualHp = value;
                }
            }
        }

        public int Strength
        {
            get
            {
                int weaponBonus = WeaponManager.GetStrengthBonus();
                int effectBonus = ActiveEffectManager.GetTotalStatBonus(EffectTypeEnum.BuffStrength);
                return BaseStrength + weaponBonus + effectBonus;
            }
        }

        public int Agility
        {
            get
            {
                return BaseAgility + ActiveEffectManager.GetTotalStatBonus(EffectTypeEnum.BuffAgility);

            }
        }

        public int MaxHp
        {
            get
            {
                int buffHp = ActiveEffectManager.GetTotalStatBonus(EffectTypeEnum.BuffMaxHp);
                return BaseMaxHp + buffHp;
            }
        }

        public void ApplySleep()
        {
            IsHeroSleep = true;
        }

        public void WakeUp()
        {
            IsHeroSleep = false;
        }
    }
}
