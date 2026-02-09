namespace ProjectTeam01.domain.Characters
{
    internal class Ogre : Enemy
    {
        public Ogre(int posX, int posY) : base(EnemyTypeEnum.Ogre, posX, posY)
        {
            ActualHp = 130;
            BaseAgility = 1;
            BaseStrength = 4;
            HostilityLevel = 3;
        }

        public Ogre(int posX, int posY, int actualHp) : base(EnemyTypeEnum.Ogre, posX, posY)
        {
            ActualHp = actualHp;
            BaseAgility = 1;
            BaseStrength = 4;
            HostilityLevel = 3;
        }

        public bool OgreCooldown { get; set; } = false;
    }
}
