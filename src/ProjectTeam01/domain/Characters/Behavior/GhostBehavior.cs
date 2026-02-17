using ProjectTeam01.domain.Combat;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class GhostBehavior(Enemy enemy, IMapQuery map) : EnemyBehavior(enemy, map)
    {
        private const int InvisibilityChancePercent = 40; // 40% шанс стать невидимым каждый тик
        
        public override void Tick(Hero hero)
        {
            int distanceToHero = DistanceToHero(hero);
            
            if (distanceToHero == 1)
            {
                Enemy.IsTriggered = true;
                if (Enemy is Ghost ghost)
                {
                    ghost.IsInvisible = false;
                }
                Attack(hero);
                return;
            }
            
            if (distanceToHero <= Enemy.HostilityLevel)
            {
                Enemy.IsTriggered = true;
                if (Enemy is Ghost ghost)
                {
                    if (Chance(InvisibilityChancePercent))
                    {
                        ghost.IsInvisible = !ghost.IsInvisible;
                    }
                }

                if (!MoveTowards(hero))
                    Teleport();
            }
            else
            {
                if (Enemy is Ghost ghost)
                {
                    if (Chance(InvisibilityChancePercent))
                    {
                        ghost.IsInvisible = !ghost.IsInvisible;
                    }
                }
                Teleport();
            }
        }

        private bool Teleport()
        {
            var room = Map.FindRoomAt(Enemy.Position.X, Enemy.Position.Y);
            if (room != null)
            {
                return TeleportInRoom(room);
            }

            var corridor = Map.FindCorridorAt(Enemy.Position.X, Enemy.Position.Y);
            if (corridor != null)
            {
                return TeleportInCorridor(corridor);
            }

            return false;
        }

        private bool TeleportInRoom(Room room)
        {
            const int MAX_TRIES = 10;
            for (int attempt = 0; attempt < MAX_TRIES; attempt++)
            {
                int minX = room.TopLeft.X + 1;
                int maxX = room.BottomRight.X - 1;
                int minY = room.TopLeft.Y + 1;
                int maxY = room.BottomRight.Y - 1;

                if (minX >= maxX) minX = maxX = room.TopLeft.X;
                if (minY >= maxY) minY = maxY = room.TopLeft.Y;

                int targetX = random.Next(minX, maxX + 1);
                int targetY = random.Next(minY, maxY + 1);

                if (TryMoveTo(targetX, targetY))
                    return true;
            }

            return false;
        }

        private bool TeleportInCorridor(Corridor corridor)
        {
            if (corridor.Cells == null || corridor.Cells.Count == 0)
                return false;

            const int MAX_TRIES = 10;
            for (int attempt = 0; attempt < MAX_TRIES; attempt++)
            {
                int randomIndex = random.Next(0, corridor.Cells.Count);
                var targetPos = corridor.Cells[randomIndex];

                if (TryMoveTo(targetPos.X, targetPos.Y))
                    return true;
            }

            return false;
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
