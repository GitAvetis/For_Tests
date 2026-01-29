using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class OgreBehavior(Ogre ogre, IMapQuery map) : EnemyBehavior(ogre, map)
    {
        public override void Tick(Hero hero)
        {
            int distanceToHero = DistanceToHero(hero);
            if (distanceToHero <= Enemy.HostilityLevel)
            {
                Enemy.IsTriggered = true;

                if (MoveTwice(hero))
                {
                }
                else if (!MoveTowards(hero))
                {
                    MoveRandom();
                }
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

        protected bool MoveTwice(Hero hero)
        {
            int dx = hero.Position.X > Enemy.Position.X ? 2 :
                     hero.Position.X < Enemy.Position.X ? -2 : 0;

            int dy = hero.Position.Y > Enemy.Position.Y ? 2 :
                     hero.Position.Y < Enemy.Position.Y ? -2 : 0;

            return TryMoveTo(Enemy.Position.X + dx, Enemy.Position.Y + dy);
        }

        public override void TakeDamage(int damageValue)
        {
            Enemy.TakeDamage(damageValue);
        }

    }
}
