using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Characters.Behavior
{
    internal class GhostBehavior(Enemy enemy, IMapQuery map) : EnemyBehavior(enemy, map)
    {
        public override void Tick(Hero hero)
        {
            int distanceToHero = DistanceToHero(hero);
            if (distanceToHero <= Enemy.HostilityLevel)
            {
                Enemy.IsTriggered = true;
                if (Enemy is Ghost ghost)
                {
                    ghost.IsInvisible = false;
                }

                if (!MoveTowards(hero))
                    Teleport();
            }
            else
            {
                Teleport();
            }

            if (DistanceToHero(hero) == 1)
                Attack(hero);
        }

        private bool Teleport()
        {
            // Сначала проверяем, в комнате ли призрак
            var room = Map.FindRoomAt(Enemy.Position.X, Enemy.Position.Y);
            if (room != null)
            {
                return TeleportInRoom(room);
            }

            // Если не в комнате, проверяем, в коридоре ли
            var corridor = Map.FindCorridorAt(Enemy.Position.X, Enemy.Position.Y);
            if (corridor != null)
            {
                return TeleportInCorridor(corridor);
            }

            // Если ни в комнате, ни в коридоре - не телепортируемся
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
                // Выбираем случайную клетку из коридора
                int randomIndex = random.Next(0, corridor.Cells.Count);
                var targetPos = corridor.Cells[randomIndex];

                if (TryMoveTo(targetPos.X, targetPos.Y))
                    return true;
            }

            return false;
        }
    }
}
