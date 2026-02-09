using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Characters.Behavior;
using ProjectTeam01.domain.Combat;
using ProjectTeam01.domain.Effects;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.domain.Session;
// отвечает за логику боя
internal partial class GameSession
{
    /// Обработать бой с врагом
    public CombatResult ProcessCombat(Enemy enemy)
    {
        if (enemy == null || enemy.IsDead) return CombatResult.NoCombat;

        // Игрок атакует врага через поведение
        var heroBehavior = new HeroBehavior(Player);
        bool playerHit = heroBehavior.Attack(enemy);
        _statistics.RecordPlayerHitAttempt(playerHit);

        if (enemy.IsDead)
        {
            int lootValue = CalculateTreasureValue(enemy);
            var treasure = new Treasure(enemy.Position.X, enemy.Position.Y, _currentLevel.LevelNumber) { Price = lootValue };
            Player.HeroBackpack.Add(treasure);
            _statistics.RecordTreasureCollected(lootValue);
            _statistics.RecordEnemyDefeated();

            _currentLevel.RemoveEntity(enemy);

            return CombatResult.EnemyDefeated;
        }

        // Враг атакует игрока через поведение
        var enemyBehavior = CreateEnemyBehavior(enemy);
        if (enemyBehavior != null)
        {
            int hpBefore = Player.ActualHp;
            bool enemyHit = enemyBehavior.Attack(Player);
            
            if (enemyHit && Player.ActualHp < hpBefore)
            {
                _statistics.RecordHitTaken();
                
                enemyBehavior.SpecialEffectOnAttack(Player);
                
                if (enemy is Ghost ghost)
                {
                    ghost.IsInvisible = false;
                }
            }
        }

        if (Player.IsDead)
        {
            return CombatResult.PlayerDefeated;
        }

        return CombatResult.Ongoing;
    }


    /// Расчет сокровищ за врага
    private int CalculateTreasureValue(Enemy enemy)
    {
        double lootValue = enemy.BaseAgility * BattleConstants.LootAgilityFactor
            + enemy.ActualHp * BattleConstants.LootHpFactor
            + enemy.BaseStrength * BattleConstants.LootStrengthFactor
            + _random.Next(0, BattleConstants.LootRandomMax + 1);
        
        int baseLoot = (int)Math.Round(lootValue);
        
        double bonusValue = baseLoot * _currentLevel.LevelNumber * 0.05;
        int levelBonus = (int)Math.Round(bonusValue);
        return baseLoot + levelBonus;
    }

}


