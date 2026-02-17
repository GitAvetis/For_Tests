namespace ProjectTeam01.domain.Characters
{
    internal class Ghost : Enemy
    {
        public Ghost(int posX, int posY) : base(EnemyTypeEnum.Ghost, posX, posY)
        {
            ActualHp = 80;
            BaseAgility = 3;
            BaseStrength = 1;
            HostilityLevel = 2;
        }

        public Ghost(int posX, int posY, int actualHp) : base(EnemyTypeEnum.Ghost, posX, posY)
        {
            ActualHp = actualHp;
            BaseAgility = 3;
            BaseStrength = 1;
            HostilityLevel = 2;
        }

        public bool IsInvisible { get; set; } = true;

    }
}
