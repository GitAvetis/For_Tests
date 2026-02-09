using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Effects;
using ProjectTeam01.domain.Items.Interfaces;

namespace ProjectTeam01.domain.Items
{
    internal class Elixir : Item, IUsableItem
    {
        private const int MaxPercentAgilityIncrease = 10;
        private const int MaxPercentStrengthIncrease = 10;
        private const int MaxPercentMaxHpIncrease = 40; // Увеличено с 20% до 40% для более заметного эффекта
        private const int MinElixirDurationSeconds = 30;
        private const int MaxElixirDurationSeconds = 60;

        public Elixir(EffectTypeEnum elixirType, int posX, int posY) : base(ItemType.Elixir, posX, posY)
        {
            ElixirType = elixirType;
        }

        void IUsableItem.Use(Hero hero)
        {
            if (!hero.HeroBackpack.AllItems.Contains(this))
                return;

            var random = Random.Shared;

            int value = CalculateValue(hero, random);
            int durationSeconds = random.Next(MinElixirDurationSeconds, MaxElixirDurationSeconds + 1);
            int durationTicks = durationSeconds;

            ActiveEffect effect = new ActiveEffect(ElixirType, value, durationTicks);
            hero.ActiveEffectManager.AddActiveEffect(effect);
        }

        private int CalculateValue(Hero hero, Random random)
        {
            int maxIncrease = ElixirType switch
            {
                EffectTypeEnum.BuffAgility => hero.BaseAgility * MaxPercentAgilityIncrease / 100,
                EffectTypeEnum.BuffStrength => hero.BaseStrength * MaxPercentStrengthIncrease / 100,
                EffectTypeEnum.BuffMaxHp => hero.MaxHp * MaxPercentMaxHpIncrease / 100,
                _ => 1
            };

            if (ElixirType == EffectTypeEnum.BuffMaxHp)
            {
                int minIncrease = Math.Max(10, maxIncrease / 3); 
                return random.Next(minIncrease, maxIncrease + 1);
            }
            return random.Next(1, Math.Max(2, maxIncrease + 1));
        }

        public EffectTypeEnum ElixirType { get; }
    }
}
