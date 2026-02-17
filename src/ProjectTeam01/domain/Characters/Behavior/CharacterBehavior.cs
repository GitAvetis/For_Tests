using ProjectTeam01.domain.Combat;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal abstract class CharacterBehavior : ICharacterBehavior
    {
        protected readonly Character Character;

        public CharacterBehavior(Character character)
        {
            Character = character;
        }

        public virtual void TakeDamage(int damageValue)
        {
            Character.TakeDamage(damageValue);
        }

        public virtual bool Attack(Character target)
        {
            if (BattleService.HitSuccess(Character.BaseAgility, target.BaseAgility))
            {
                target.TakeDamage(Character.BaseStrength);
                return true;
            }
            else return false;
        }
    }
}
