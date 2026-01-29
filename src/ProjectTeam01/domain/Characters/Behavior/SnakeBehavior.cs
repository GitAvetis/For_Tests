using ProjectTeam01.domain.Effects;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class SnakeBehavior(Enemy enemy, IMapQuery map) : EnemyBehavior(enemy, map)
    {
        private int dx = 1;
        private int dy = 1;

        public override void Tick(Hero hero)
        {
            int distanceToHero = DistanceToHero(hero);

            if (distanceToHero <= Enemy.HostilityLevel)
            {
                Enemy.IsTriggered = true;
                if (!MoveTowards(hero))
                    ChangeDirection();
            }
            else
            {
                ChangeDirection();
            }

            if (DistanceToHero(hero) == 1)
            {
                if (Attack(hero)) SpecialEffectOnAttack(hero);
            }
        }

        private void ChangeDirection()
        {
            int attempts = 8;
            while (attempts-- > 0)
            {
                if (Chance(50)) dx *= -1;
                if (Chance(50)) dy *= -1;

                if (TryMoveTo(Enemy.Position.X + dx, Enemy.Position.Y + dy))
                    break;
            }
        }

        protected static void FallHeroToSleep(Hero hero)
        {
            if (hero.IsHeroSleep) return;
            ActiveEffect sleep = new(EffectTypeEnum.Sleep);
            hero.ActiveEffectManager.AddActiveEffect(sleep);
        }

        protected override void SpecialEffectOnAttack(Hero hero)
        {
            if (Chance(Snake.SleepChancePercent))
                FallHeroToSleep(hero);
        }
    }
}
