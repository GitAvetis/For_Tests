using ProjectTeam01.domain.Combat;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class VampireBehavior(Vampire vampire, IMapQuery map) : EnemyBehavior(vampire, map)
    {
        public override void Tick(Hero hero)
        {
            int distanceToHero = DistanceToHero(hero);

            if (distanceToHero == 1)
            {
                Enemy.IsTriggered = true;
                Attack(hero);
                return;
            }

            if (distanceToHero <= Enemy.HostilityLevel)
            {
                Enemy.IsTriggered = true;

                if (!MoveTowards(hero))
                    MoveRandom();
            }
            else
            {
                MoveRandom();
            }
        }

        public override bool Attack(Character target)
        {
            if (target is not Hero hero)
                return false;

            if (BattleService.HitSuccess(Enemy.BaseAgility, target.BaseAgility))
            {
                int damage = EnemyDamageCalculator.CalculateDamage(Enemy, hero);
                target.TakeDamage(damage);
                return true;
            }
            return false;
        }

        public override void TakeDamage(int damageValue)
        {
            if (!vampire.EvadedFirstAttack)
                vampire.EvadedFirstAttack = true;
            else Enemy.TakeDamage(damageValue);
        }
    }
}
