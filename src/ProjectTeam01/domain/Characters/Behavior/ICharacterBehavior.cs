namespace ProjectTeam01.domain.Characters.Behavior
{
    internal interface ICharacterBehavior
    {
        public bool Attack(Character character);
        public void TakeDamage(int damageValue);
    }
}
