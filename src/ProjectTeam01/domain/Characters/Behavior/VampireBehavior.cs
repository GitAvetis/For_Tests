using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class VampireBehavior(Vampire vampire, IMapQuery map) : EnemyBehavior(vampire, map)
    {
        public override void Tick(Hero hero)
        {
            int distanceToHero = DistanceToHero(hero);

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

            if (DistanceToHero(hero) == 1)
            {
                Attack(hero);
            }
        }

        public override void TakeDamage(int damageValue)
        {
            if (!vampire.EvadedFirstAttack)
                vampire.EvadedFirstAttack = true;
            else Enemy.TakeDamage(damageValue);
        }
    }
}
