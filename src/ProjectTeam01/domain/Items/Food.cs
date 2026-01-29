using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Items.Interfaces;

namespace ProjectTeam01.domain.Items
{
    internal class Food : Item, IUsableItem
    {
        /// Количество HP, которое восстанавливает эта еда
        public int HealthValue { get; }

        public Food(int healthValue, int posX, int posY) : base(ItemType.Food, posX, posY)
        {
            HealthValue = healthValue;
        }

        void IUsableItem.Use(Hero hero)
        {
            // Восстанавливаем HP, но не больше максимума
            int healAmount = HealthValue;
            int missingHp = hero.MaxHp - hero.ActualHp;
            if (healAmount > missingHp)
                healAmount = missingHp;
            
            hero.Heal(healAmount);
        }
    }
}
