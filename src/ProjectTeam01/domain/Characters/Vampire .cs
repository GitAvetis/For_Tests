namespace ProjectTeam01.domain.Characters
{
    internal class Vampire : Enemy
    {
        public Vampire(int posX, int posY) : base(EnemyTypeEnum.Vampire, posX, posY)
        {
            ActualHp = 110;
            BaseAgility = 3;
            BaseStrength = 2;
            HostilityLevel = 4;
        }

        public Vampire(int posX, int posY, int actualHp) : base(EnemyTypeEnum.Vampire, posX, posY)// конструктор для воостановления врага из сохранения
        {
            ActualHp = actualHp;
            BaseAgility = 3;
            BaseStrength = 2;
            HostilityLevel = 4;
        }

        public bool EvadedFirstAttack { get; set; } = false;
    }
}
