using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal abstract class EnemyBehavior(Enemy enemy, IMapQuery map) : CharacterBehavior(enemy)
    {
        protected readonly Enemy Enemy = enemy;
        protected readonly IMapQuery Map = map;
        protected static readonly Random random = new();
        protected bool LootDropped { get; set; } = false;

        public abstract void Tick(Hero hero);

        protected virtual void SpecialEffectOnAttack(Hero hero) { }

        protected bool MoveRandom()
        {
            for (int attempts = 0; attempts < 10; attempts++)
            {
                int dx = random.Next(-1, 2);
                int dy = random.Next(-1, 2);
                if (TryMoveTo(Enemy.Position.X + dx, Enemy.Position.Y + dy)) return true;
            }
            return false;
        }

        protected bool MoveTowards(Hero hero)
        {
            int bestX = Enemy.Position.X;
            int bestY = Enemy.Position.Y;
            int bestDist = Map.GetDistance(bestX, bestY, hero.Position.X, hero.Position.Y);

            bool found = false;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = Enemy.Position.X + dx;
                    int ny = Enemy.Position.Y + dy;

                    if (Map.IsOccupied(nx, ny))
                        continue;

                    int dist = Map.GetDistance(nx, ny, hero.Position.X, hero.Position.Y);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestX = nx;
                        bestY = ny;
                        found = true;
                    }
                }
            }

            return found && TryMoveTo(bestX, bestY);
        }

        protected int DistanceToHero(Hero hero)
        {
            return Map.GetDistance(Enemy.Position.X, Enemy.Position.Y, hero.Position.X, hero.Position.Y);
        }

        protected bool TryMoveTo(int x, int y)
        {
            if (!Map.IsOccupied(x, y))
            {
                Enemy.MoveTo(x, y);
                return true;
            }
            else
                return false;
        }

        static protected bool Chance(int percent)
        {
            return random.Next(0, 100) < percent;
        }
    }
}
