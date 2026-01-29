namespace ProjectTeam01.domain.Characters
{
    internal class Snake : Enemy
    {
        public const int SleepChancePercent = 30;

        public Snake(int posX, int posY) : base(EnemyTypeEnum.Snake, posX, posY)
        {
            ActualHp = 100;
            BaseAgility = 4;
            BaseStrength = 1;
            HostilityLevel = 3;
        }

        public Snake(int posX, int posY, int actualHp) : base(EnemyTypeEnum.Snake, posX, posY)
        {
            ActualHp = actualHp;
            BaseAgility = 4;
            BaseStrength = 1;
            HostilityLevel = 3;
        }

    }
}
