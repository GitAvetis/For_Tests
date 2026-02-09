namespace ProjectTeam01.domain.Characters
{
    /// Представление Mimic для использования в ViewModels
    public enum MimicsRepresentation
    {
        Food,
        Elixir,
        Scroll,
        Weapon,
        Mimic
    }

    internal class Mimic : Enemy
    {
        protected static readonly Random random = new();
        public MimicsRepresentation Representation { get; set; }

        public Mimic(int posX, int posY) : base(EnemyTypeEnum.Mimic, posX, posY)
        {
            ActualHp = 110;
            BaseAgility = 3;
            BaseStrength = 1;
            HostilityLevel = 2;
            Representation = RandomRepresentation();
        }

        public Mimic(int posX, int posY, int actualHp) : base(EnemyTypeEnum.Mimic, posX, posY)
        {
            ActualHp = actualHp;
            BaseAgility = 3;
            BaseStrength = 1;
            HostilityLevel = 2;
            Representation = RandomRepresentation();
        }

        private MimicsRepresentation RandomRepresentation()
        {
            MimicsRepresentation mimics = (MimicsRepresentation)random.Next(0, 4);
            return mimics;
        }
    }
}
