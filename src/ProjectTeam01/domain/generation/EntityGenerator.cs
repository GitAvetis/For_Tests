using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Effects;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.domain.generation;

/// Генератор сущностей для размещения на уровне
internal class EntityGenerator
{
    private readonly Random _random;

    public EntityGenerator()
    {
        _random = new Random();
    }

    /// Разместить игрока в стартовой комнате
    public Hero PlacePlayer(Level level)
    {
        if (level == null) throw new ArgumentNullException(nameof(level));

        var hero = new Hero(level.StartPosition.X, level.StartPosition.Y);
        level.AddEntity(hero);

        return hero;
    }

    /// Разместить уже существующего героя на новом уровне (для перехода между уровнями).
    public void PlaceExistingHero(Level level, Hero hero)
    {
        if (level == null) throw new ArgumentNullException(nameof(level));
        if (hero == null) throw new ArgumentNullException(nameof(hero));

        hero.MoveTo(level.StartPosition.X, level.StartPosition.Y);
        level.AddEntity(hero);
    }

    /// Разместить врагов на уровне
    public void PlaceEnemies(Level level, int levelNumber, float difficulty = 1.0f)
    {
        if (level == null) throw new ArgumentNullException(nameof(level));

        int baseEnemyCount = 3 + levelNumber / 2;
        int enemyCount = _random.Next(baseEnemyCount, baseEnemyCount + 5);

        var nonStartRooms = level.Rooms.Where(r => !r.IsStartRoom).ToList();
        if (nonStartRooms.Count == 0) return;

        for (int i = 0; i < enemyCount; i++)
        {
            var room = nonStartRooms[_random.Next(nonStartRooms.Count)];

            var position = GetRandomPositionInRoom(room);

            if (level.HasAnyEntityAt(position.X, position.Y) || 
                (position.X == level.ExitPosition.X && position.Y == level.ExitPosition.Y) ||
                (position.X == level.StartPosition.X && position.Y == level.StartPosition.Y))
            {
                position = FindFreePositionInRoom(room, level);
                if (position.X == -1) continue;
            }

            var enemy = CreateRandomEnemy(position.X, position.Y);
            enemy.ApplyDifficulty(difficulty);//корректируем сложность
            level.AddEntity(enemy);
        }
    }

    /// Разместить предметы на уровне
    public void PlaceItems(Level level, int levelNumber, float difficulty = 1.0f)
    {
        if (level == null) throw new ArgumentNullException(nameof(level));

        int baseItemCount = 10 - levelNumber / 3;
        int itemCount = Math.Max(3, _random.Next(baseItemCount, baseItemCount + 5));
        itemCount =(int)(itemCount * difficulty);

        var nonStartRooms = level.Rooms.Where(r => !r.IsStartRoom).ToList();
        if (nonStartRooms.Count == 0) return;

        for (int i = 0; i < itemCount; i++)
        {
            var room = nonStartRooms[_random.Next(nonStartRooms.Count)];

            var position = GetRandomPositionInRoom(room);

            if (level.HasAnyEntityAt(position.X, position.Y) || 
                (position.X == level.ExitPosition.X && position.Y == level.ExitPosition.Y) ||
                (position.X == level.StartPosition.X && position.Y == level.StartPosition.Y))
            {
                position = FindFreePositionInRoom(room, level);
                if (position.X == -1) continue;
            }

            var item = CreateRandomItem(position.X, position.Y, levelNumber);
            level.AddEntity(item);
        }
    }

    /// Получить случайную позицию в комнате
    private Position GetRandomPositionInRoom(Room room)
    {
        int minX = room.TopLeft.X + 1;
        int maxX = room.BottomRight.X - 1;
        int minY = room.TopLeft.Y + 1;
        int maxY = room.BottomRight.Y - 1;

        if (minX >= maxX) minX = maxX = room.TopLeft.X;
        if (minY >= maxY) minY = maxY = room.TopLeft.Y;

        int x = _random.Next(minX, maxX + 1);
        int y = _random.Next(minY, maxY + 1);

        return new Position(x, y);
    }

    /// Найти свободную позицию в комнате
    private Position FindFreePositionInRoom(Room room, Level level)
    {
        int minX = room.TopLeft.X + 1;
        int maxX = room.BottomRight.X - 1;
        int minY = room.TopLeft.Y + 1;
        int maxY = room.BottomRight.Y - 1;

        if (minX >= maxX) minX = maxX = room.TopLeft.X;
        if (minY >= maxY) minY = maxY = room.TopLeft.Y;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            int x = _random.Next(minX, maxX + 1);
            int y = _random.Next(minY, maxY + 1);

            if (!level.HasAnyEntityAt(x, y) && 
                !(x == level.ExitPosition.X && y == level.ExitPosition.Y))
            {
                return new Position(x, y);
            }
        }

        return new Position(-1, -1);
    }

    /// Создать случайного врага
    private Enemy CreateRandomEnemy(int x, int y)
    {
        var enemyType = (EnemyTypeEnum)_random.Next(0, 6);

        Enemy enemy = enemyType switch
        {
            EnemyTypeEnum.Zombie => new Zombie(x, y),
            EnemyTypeEnum.Vampire => new Vampire(x, y),
            EnemyTypeEnum.Ghost => new Ghost(x, y),
            EnemyTypeEnum.Ogre => new Ogre(x, y),
            EnemyTypeEnum.Snake => new Snake(x, y),
            EnemyTypeEnum.Mimic => new Mimic(x, y),
            _ => new Zombie(x, y)
        };
        return enemy;
    }

    /// Создать случайный предмет
    private Item CreateRandomItem(int x, int y, int levelNumber)
    {
        int roll = _random.Next(0, 100);

        if (roll < 35)
        {
            int healthValue = _random.Next(5, 21);
            return new Food(healthValue, x, y);
        }
        else if (roll < 60)
        {
            var elixirType = (EffectTypeEnum)_random.Next(0, 3);
            return new Elixir(elixirType, x, y);
        }
        else if (roll < 80)
        {
            var scrollType = (ScrollTypeEnum)_random.Next(0, 3);
            return new Scroll(scrollType, x, y);
        }
        else
        {
            var weaponType = (WeaponTypeEnum)_random.Next(0, 4);
            return new Weapon(weaponType, x, y);
        }
    }
}

