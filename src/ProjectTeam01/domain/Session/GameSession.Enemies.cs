using System.Linq;
using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Characters.Behavior;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain.Session;
// отвечает за логику хода врагов
internal partial class GameSession
{
    /// Обработать ход врагов
    public void ProcessEnemiesTurn(Enemy? skipEnemy = null)
    {
        _mapQuery = new LevelMapQuery(_currentLevel);

        foreach (var enemy in _currentLevel.GetEnemies().ToList())
        {
            if (skipEnemy != null && ReferenceEquals(enemy, skipEnemy))
                continue;

            if (!enemy.IsDead)
            {
                var behavior = CreateEnemyBehavior(enemy);
                if (behavior == null) continue;

                int hpBefore = Player.ActualHp;
                var oldPos = enemy.Position;

                behavior.Tick(Player);

                if (!enemy.Position.Equals(oldPos))
                    _currentLevel.UpdateEntityPosition(enemy, oldPos);

                if (Player.ActualHp < hpBefore)
                    _statistics.RecordHitTaken();

                if (Player.IsDead) break; // Остановить обработку, если игрок умер
            }
        }
    }

    private EnemyBehavior? CreateEnemyBehavior(Enemy enemy)
    {
        return enemy.EnemyType switch
        {
            EnemyTypeEnum.Zombie when enemy is Zombie z => new ZombieBehavior(z, _mapQuery),
            EnemyTypeEnum.Vampire when enemy is Vampire v => new VampireBehavior(v, _mapQuery),
            EnemyTypeEnum.Ghost when enemy is Ghost g => new GhostBehavior(g, _mapQuery),
            EnemyTypeEnum.Ogre when enemy is Ogre o => new OgreBehavior(o, _mapQuery),
            EnemyTypeEnum.Snake when enemy is Snake s => new SnakeBehavior(s, _mapQuery),
            EnemyTypeEnum.Mimic when enemy is Mimic m => new MimicBehavior(m, _mapQuery),
            _ => null
        };
    }
}


