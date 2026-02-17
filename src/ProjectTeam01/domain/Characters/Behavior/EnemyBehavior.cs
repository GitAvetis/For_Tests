using System;
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

        public virtual void SpecialEffectOnAttack(Hero hero) { }

        protected bool MoveRandom()
        {
            bool inCorridor = IsInCorridor();
            
            for (int attempts = 0; attempts < 10; attempts++)
            {
                int dx, dy;
                
                if (inCorridor)
                {
                    // В корридоре двигаемся только по прямой (горизонтально или вертикально)
                    if (random.Next(0, 2) == 0)
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
                    // В комнате можно двигаться в любом направлении
                    dx = random.Next(-1, 2);
                    dy = random.Next(-1, 2);
                }
                
                if (dx == 0 && dy == 0) continue;
                
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
            bool inCorridor = IsInCorridor();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    
                    // В корридоре разрешаем только прямые движения (не диагональные)
                    if (inCorridor && dx != 0 && dy != 0)
                        continue;

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

            // Если нашли оптимальную клетку, пытаемся туда переместиться
            if (found && TryMoveTo(bestX, bestY))
                return true;

            // Fallback: если MoveTowards не сработал (например, на поворотах корридора),
            // пробуем двигаться в направлении игрока простым способом
            return TryMoveTowardsPlayerSimple(hero);
        }

        /// Простое движение в направлении игрока (fallback для MoveTowards)
        private bool TryMoveTowardsPlayerSimple(Hero hero)
        {
            bool inCorridor = IsInCorridor();
            int heroX = hero.Position.X;
            int heroY = hero.Position.Y;
            int enemyX = Enemy.Position.X;
            int enemyY = Enemy.Position.Y;

            // Определяем направление к игроку
            int dirX = heroX > enemyX ? 1 : heroX < enemyX ? -1 : 0;
            int dirY = heroY > enemyY ? 1 : heroY < enemyY ? -1 : 0;

            if (inCorridor)
            {
                // В корридоре пробуем сначала направление с большим смещением
                int diffX = Math.Abs(heroX - enemyX);
                int diffY = Math.Abs(heroY - enemyY);
                
                if (diffX > diffY)
                {
                    // Горизонтальное направление приоритетнее
                    if (dirX != 0 && TryMoveTo(enemyX + dirX, enemyY))
                        return true;
                    if (dirY != 0 && TryMoveTo(enemyX, enemyY + dirY))
                        return true;
                }
                else
                {
                    // Вертикальное направление приоритетнее
                    if (dirY != 0 && TryMoveTo(enemyX, enemyY + dirY))
                        return true;
                    if (dirX != 0 && TryMoveTo(enemyX + dirX, enemyY))
                        return true;
                }
            }
            else
            {
                // В комнате пробуем все направления к игроку
                if (dirX != 0 && TryMoveTo(enemyX + dirX, enemyY))
                    return true;
                if (dirY != 0 && TryMoveTo(enemyX, enemyY + dirY))
                    return true;
                if (dirX != 0 && dirY != 0 && TryMoveTo(enemyX + dirX, enemyY + dirY))
                    return true;
            }

            return false;
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
        
        /// Проверить, находится ли враг в корридоре
        protected bool IsInCorridor()
        {
            return Map.FindCorridorAt(Enemy.Position.X, Enemy.Position.Y) != null;
        }
    }
}
