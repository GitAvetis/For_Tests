using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class MimicBehavior(Mimic mimic, IMapQuery map) : EnemyBehavior(mimic, map)
    {
        public override void Tick(Hero hero)
        {
            int distanceToHero = DistanceToHero(hero);
            if (distanceToHero <= Enemy.HostilityLevel)
            {
                Enemy.IsTriggered = true;
                mimic.Representation = Items.ItemType.Mimic;

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
