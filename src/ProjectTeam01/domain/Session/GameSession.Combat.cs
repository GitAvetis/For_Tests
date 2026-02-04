using ProjectTeam01.domain.Characters;
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

        // Игрок атакует врага
        bool playerHit = BattleService.HitSuccess(Player.Agility, enemy.BaseAgility);
        if (enemy is Vampire vampire && !vampire.EvadedFirstAttack)
        {
            vampire.EvadedFirstAttack = true;
            playerHit = false;
        }
        _statistics.RecordPlayerHitAttempt(playerHit);
        if (playerHit)
        {
            int playerDamage = CalculatePlayerDamage();
            enemy.TakeDamage(playerDamage);
        }

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

        bool alwaysHit = enemy is Ogre;
        bool enemyHit = BattleService.HitSuccess(enemy.BaseAgility, Player.Agility, alwaysHit);
        if (enemyHit)
        {
            int enemyDamage = CalculateEnemyDamage(enemy);
            Player.TakeDamage(enemyDamage);
            _statistics.RecordHitTaken();
            ApplyEnemySpecialOnHit(enemy);
        }

        if (Player.IsDead)
        {
            return CombatResult.PlayerDefeated;
        }

        return CombatResult.Ongoing;
    }

    /// Рассчитать урон игрока
    private int CalculatePlayerDamage()
    {
        // Если есть оружие: weapon_strength * (strength + 65) / 100
        if (Player.WeaponManager.EquippedWeapon != null)
        {
            int weaponStrength = Player.WeaponManager.EquippedWeapon.StrengthBonus;
            int totalStrength = Player.BaseStrength + Player.ActiveEffectManager.GetTotalStatBonus(EffectTypeEnum.BuffStrength);
            return (int)Math.Round(weaponStrength * (totalStrength + BattleConstants.StrengthAddition) / 100.0);
        }
        // Без оружия: 30 + (strength - 50) * 0.3
        else
        {
            int totalStrength = Player.BaseStrength + Player.ActiveEffectManager.GetTotalStatBonus(EffectTypeEnum.BuffStrength);
            return (int)Math.Round(BattleConstants.InitialDamage
                + (totalStrength - BattleConstants.StandardStrength) * BattleConstants.StrengthFactor);
        }
    }

    /// Рассчитать урон врага (формула из rogue_sample)
    private int CalculateEnemyDamage(Enemy enemy)
    {
        return enemy switch
        {
            Vampire => CalculateVampireDamage(),
            Ogre => CalculateOgreDamage(enemy),
            _ => CalculateStandardEnemyDamage(enemy)
        };
    }

    /// Урон стандартных врагов 30 + (strength - 50) * 0.3
    private int CalculateStandardEnemyDamage(Enemy enemy)
    {
        return (int)Math.Round(BattleConstants.InitialDamage
            + (enemy.BaseStrength - BattleConstants.StandardStrength) * BattleConstants.StrengthFactor);
    }

    /// Урон вампира: max_hp / 10 (отнимает часть максимального HP)
    private int CalculateVampireDamage()
    {
        return Player.MaxHp / BattleConstants.MaxHpPart;
    }

    /// Урон огра: (strength - 50) * 0.3 (без базового урона, раз в два хода)
    private int CalculateOgreDamage(Enemy ogre)
    {
        if (ogre is not Ogre ogreEnemy)
            return 0;

        int damage = 0;
        if (!ogreEnemy.OgreCooldown)
        {
            damage = (int)Math.Round((ogre.BaseStrength - BattleConstants.StandardStrength) * BattleConstants.StrengthFactor);
            ogreEnemy.OgreCooldown = true;
        }
        else
        {
            ogreEnemy.OgreCooldown = false;
        }

        return damage;
    }

    /// Расчет сокровищ за врага
    private int CalculateTreasureValue(Enemy enemy)
    {
        // loot = agility * 0.2 + health * 0.5 + strength * 0.5 + random(0, 20)
        int baseLoot = (int)Math.Round(
            enemy.BaseAgility * BattleConstants.LootAgilityFactor
            + enemy.ActualHp * BattleConstants.LootHpFactor
            + enemy.BaseStrength * BattleConstants.LootStrengthFactor
            + _random.Next(0, BattleConstants.LootRandomMax + 1)
        );

        int levelBonus = (int)Math.Round(baseLoot * _currentLevel.LevelNumber * 0.05);
        return baseLoot + levelBonus;
    }

    private void ApplyEnemySpecialOnHit(Enemy enemy)
    {
        switch (enemy)
        {
            case Ghost ghost:
                ghost.IsInvisible = false;
                break;
            case Ogre ogre:
                break;
            case Snake:
                if (_random.Next(0, 100) < Snake.SleepChancePercent)
                    Player.ActiveEffectManager.AddActiveEffect(new ActiveEffect(EffectTypeEnum.Sleep));
                break;
        }
    }
}


