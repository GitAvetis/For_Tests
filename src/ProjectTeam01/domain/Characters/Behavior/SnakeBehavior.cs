using System;
using ProjectTeam01.domain.Combat;
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

            if (distanceToHero == 1)
            {
                Enemy.IsTriggered = true;
                if (Attack(hero)) SpecialEffectOnAttack(hero);
                return;
            }

            if (distanceToHero <= Enemy.HostilityLevel)
            {
                Enemy.IsTriggered = true;
                // Всегда пытаемся двигаться к игроку кратчайшим путем
                if (!MoveTowards(hero))
                    ChangeDirection();
            }
            else
            {
                ChangeDirection();
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

        private void ChangeDirection()
        {
            bool inCorridor = IsInCorridor();
            int attempts = 8;
            
            while (attempts-- > 0)
            {
                if (inCorridor)
                {
                    // В корридоре двигаемся только по прямой
                    if (Chance(50))
                    {
                        dx = random.Next(-1, 2);
                        dy = 0;
                    }
                    else
                    {
                        dx = 0;
                        dy = random.Next(-1, 2);
                    }
                }
                else
                {
                    // В комнате можно менять направление свободно
                    if (Chance(50)) dx *= -1;
                    if (Chance(50)) dy *= -1;
                }
                
                if (dx == 0 && dy == 0) continue;

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

        public override void SpecialEffectOnAttack(Hero hero)
        {
            if (Chance(Snake.SleepChancePercent))
                FallHeroToSleep(hero);
        }
    }
}
