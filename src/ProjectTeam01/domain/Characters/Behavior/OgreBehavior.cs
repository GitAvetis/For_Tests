using System;
using ProjectTeam01.domain.Combat;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class OgreBehavior(Ogre ogre, IMapQuery map) : EnemyBehavior(ogre, map)
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
        }

        public override bool Attack(Character target)
        {
            if (target is not Hero hero)
                return false;
                
            if (BattleService.HitSuccess(Enemy.BaseAgility, target.BaseAgility))
            {
                int damage = EnemyDamageCalculator.CalculateDamage(Enemy, hero);
                if (damage > 0)
                {
                    target.TakeDamage(damage);
                    return true;
                }
            }
            return false;
        }

        protected bool MoveTwice(Hero hero)
        {
            bool inCorridor = IsInCorridor();
            
            int dx = hero.Position.X > Enemy.Position.X ? 2 :
                     hero.Position.X < Enemy.Position.X ? -2 : 0;

            int dy = hero.Position.Y > Enemy.Position.Y ? 2 :
                     hero.Position.Y < Enemy.Position.Y ? -2 : 0;
            
            // В корридоре нельзя двигаться по диагонали
            if (inCorridor && dx != 0 && dy != 0)
            {
                // Выбираем направление с большим смещением
                if (Math.Abs(hero.Position.X - Enemy.Position.X) > Math.Abs(hero.Position.Y - Enemy.Position.Y))
                {
                    dy = 0;
                }
                else
                {
                    dx = 0;
                }
            }

            return TryMoveTo(Enemy.Position.X + dx, Enemy.Position.Y + dy);
        }

        public override void TakeDamage(int damageValue)
        {
            Enemy.TakeDamage(damageValue);
        }

    }
}
