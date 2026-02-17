using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Items.Interfaces;

namespace ProjectTeam01.domain.Items
{
    public enum ScrollTypeEnum
    {
        Agility,
        Strength,
        MaxHp
    }

    internal class Scroll(ScrollTypeEnum scrollType, int posX, int posY) : Item(ItemType.Scroll, posX, posY), IUsableItem
    {
        public ScrollTypeEnum ScrollType { get; } = scrollType;

        void IUsableItem.Use(Hero hero)
        {
            switch (ScrollType)
            {
                case ScrollTypeEnum.Agility:
                    hero.ChangeBaseAgility(5);
                    break;
                case ScrollTypeEnum.Strength:
                    hero.ChangeBaseStrength(5);
                    break;
                case ScrollTypeEnum.MaxHp:
                    hero.ChangeBaseMaxHp(25);
                    hero.Heal(25);
                    break;
                default:
                    break;
            }
        }


    }

}
