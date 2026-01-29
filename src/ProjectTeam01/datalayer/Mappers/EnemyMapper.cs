using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain.Characters;

namespace ProjectTeam01.datalayer.Mappers
{
    internal class EnemyMapper
    {
        public static EnemySave ToSave(Enemy enemy)
        {
            EnemySave enemySave = new()
            {
                EnemyType = enemy.EnemyType,
                PosX = enemy.Position.X,
                PosY = enemy.Position.Y,
                ActualHp = enemy.ActualHp,
                IsTriggered = enemy.IsTriggered
            };
            if (enemy is Ogre ogre)
            {
                enemySave.OgreCooldown = ogre.OgreCooldown;
            }
            else if (enemy is Ghost ghost)
            {
                enemySave.IsInvisible = ghost.IsInvisible;
            }
            else if (enemy is Vampire vampire)
            {
                enemySave.EvadedFirstAttack = vampire.EvadedFirstAttack;
            }
            else if (enemy is Mimic mimic)
            {
                enemySave.Representation = mimic.Representation;
            }
            return enemySave;
        }

        public static Enemy FromSave(EnemySave enemySave)
        {
            switch (enemySave.EnemyType)
            {
                case EnemyTypeEnum.Zombie:
                    {
                        Zombie zombie = new Zombie(enemySave.PosX, enemySave.PosY, enemySave.ActualHp)
                        {
                            IsTriggered = enemySave.IsTriggered
                        };
                        return zombie;
                    }
                case EnemyTypeEnum.Vampire:
                    {
                        Vampire vampire = new Vampire(enemySave.PosX, enemySave.PosY, enemySave.ActualHp)
                        {
                            IsTriggered = enemySave.IsTriggered,
                            EvadedFirstAttack = enemySave.EvadedFirstAttack ?? false
                        };
                        return vampire;
                    }
                case EnemyTypeEnum.Ghost:
                    {
                        Ghost ghost = new Ghost(enemySave.PosX, enemySave.PosY, enemySave.ActualHp)
                        {
                            IsTriggered = enemySave.IsTriggered,
                            IsInvisible = enemySave.IsInvisible ?? false
                        };
                        return ghost;
                    }
                case EnemyTypeEnum.Ogre:
                    {
                        Ogre ogre = new Ogre(enemySave.PosX, enemySave.PosY, enemySave.ActualHp)
                        {
                            IsTriggered = enemySave.IsTriggered,
                            OgreCooldown = enemySave.OgreCooldown ?? false
                        };
                        return ogre;
                    }
                case EnemyTypeEnum.Snake:
                    {
                        Snake snake = new Snake(enemySave.PosX, enemySave.PosY, enemySave.ActualHp)
                        {
                            IsTriggered = enemySave.IsTriggered
                        };
                        return snake;
                    }
                case EnemyTypeEnum.Mimic:
                    {
                        Mimic mimic = new Mimic(enemySave.PosX, enemySave.PosY, enemySave.ActualHp)
                        {
                            IsTriggered = enemySave.IsTriggered,
                            Representation = enemySave.Representation ?? MimicsRepresentation.Mimic
                        };
                        return mimic;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(enemySave.EnemyType),
                        $"Unknown enemy type: {enemySave.EnemyType}");
                    }
            }
        }
    }
}
