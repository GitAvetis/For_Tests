using ProjectTeam01.domain.Items;

namespace ProjectTeam01.domain.Characters
{
    /// <summary>
    /// Представление Mimic для использования в ViewModels
    /// </summary>
    // public enum MimicsRepresentation
    // {
    //     Food,
    //     Elixir,
    //     Scroll,
    //     Weapon,
    //     Mimic
    // }

    internal class Mimic : Enemy
    {
        protected static readonly Random random = new();
        public ItemType Representation { get; set; }

        public Mimic(int posX, int posY) : base(EnemyTypeEnum.Mimic, posX, posY)
        {
            ActualHp = 175;
            BaseAgility = 3;
            BaseStrength = 1;
            HostilityLevel = 1;
            Representation = RandomRepresentation();
        }

        public Mimic(int posX, int posY, int actualHp) : base(EnemyTypeEnum.Mimic, posX, posY)
        {
            ActualHp = actualHp;
            BaseAgility = 3;
            BaseStrength = 1;
            HostilityLevel = 1;
            Representation = RandomRepresentation();
        }

        private ItemType RandomRepresentation()
        {
            ItemType mimics = (ItemType)random.Next(0, 4);
            return mimics;
        }
    }
}
