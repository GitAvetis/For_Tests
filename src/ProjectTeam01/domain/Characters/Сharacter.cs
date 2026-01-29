namespace ProjectTeam01.domain.Characters
{
    internal abstract class Character : GameObject
    {
        protected Character(int posX, int posY) : base(posX, posY) { }

        private int _agility;
        private int _strength;
        private int _baseMaxHp;

        public int BaseMaxHp
        {
            get
            {
                return _baseMaxHp;
            }
            protected set
            {
                if (value < 1)
                    _baseMaxHp = 1;
                else
                    _baseMaxHp = value;
            }
        }

        public int BaseAgility
        {
            get { return _agility; }
            protected set
            {
                if (value < 0)
                    _agility = 0;
                else
                    _agility = value;
            }
        }

        public int BaseStrength
        {
            get { return _strength; }
            protected set
            {
                if (value < 0)
                    _strength = 0;
                else
                    _strength = value;
            }
        }

        public void TakeDamage(int damageValue)
        {
            ActualHp -= damageValue;
        }

        public void ChangeBaseAgility(int value)
        {
            BaseAgility += value;
        }
        public void ChangeBaseStrength(int value)
        {
            BaseStrength += value;
        }

        public void ChangeBaseMaxHp(int value)
        {
            BaseMaxHp += value;
        }

        public void Heal(int healValue)
        {
            ActualHp += healValue;
        }

        abstract public int ActualHp { get; protected set; }
        public bool IsDead { get { return ActualHp <= 0; } }

    }
}
