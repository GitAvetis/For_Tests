using ProjectTeam01.domain.Combat;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class MimicBehavior(Mimic mimic, IMapQuery map) : EnemyBehavior(mimic, map)
    {
        public override void Tick(Hero hero)
        {
            int distanceToHero = DistanceToHero(hero);
            
            if (distanceToHero == 1)
            {
                Enemy.IsTriggered = true;
                mimic.Representation = MimicsRepresentation.Mimic;
                Attack(hero);
                return;
            }
            
            if (distanceToHero <= Enemy.HostilityLevel)
            {
                Enemy.IsTriggered = true;
                mimic.Representation = MimicsRepresentation.Mimic;

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
    }
}
