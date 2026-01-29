using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class ZombieBehavior(Enemy enemy, IMapQuery map) : EnemyBehavior(enemy, map)
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
                Attack(hero);
        }

    }
}
