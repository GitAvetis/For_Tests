namespace ProjectTeam01.domain.Characters
{
    internal class Zombie : Enemy
    {
        public Zombie(int posX, int posY) : base(EnemyTypeEnum.Zombie, posX, posY)
        {
            ActualHp = 110;
            BaseAgility = 1;
            BaseStrength = 2;
            HostilityLevel = 3;
        }
        public Zombie(int posX, int posY, int actualHp) : base(EnemyTypeEnum.Zombie, posX, posY)// конструктор для воостановления врага из сохранения
        {
            ActualHp = actualHp;
            BaseAgility = 1;
            BaseStrength = 2;
            HostilityLevel = 3;
        }
    }
}
