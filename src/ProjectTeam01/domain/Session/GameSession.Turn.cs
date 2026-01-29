using ProjectTeam01.domain;
using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Combat;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.domain.Session;
// отвечает за логику обработка хода
internal partial class GameSession
{
    /// Единый пошаговый ход: пока игрок не сделал ход — мир стоит.
    public void ProcessTurn(PlayerAction action)
    {
        if (action == null) return;
        if (IsGameOver() || IsGameCompleted) return;

        if (Player.IsHeroSleep)
        {
            Player.ActiveEffectManager.TickEffects();
            ProcessEnemiesTurn(skipEnemy: null);
            return;
        }

        Player.ActiveEffectManager.TickEffects();

        Enemy? enemyEngagedInCombat = null;

        switch (action.Type)
        {
            case PlayerActionType.Move:
                HandlePlayerMove(action.TargetX, action.TargetY, out enemyEngagedInCombat);
                break;
            case PlayerActionType.UseItem:
                if (action.SelectedItem != null)
                    UsePlayerItem(action.SelectedItem);
                break;
            case PlayerActionType.EquipWeapon:
                if (action.SelectedItem is Weapon weapon)
                    EquipPlayerWeapon(weapon);
                break;
            case PlayerActionType.UnequipWeapon:
                UnequipPlayerWeapon();
                break;
            case PlayerActionType.DropItem:
                if (action.SelectedItem != null)
                    DropPlayerItem(action.SelectedItem);
                break;
            case PlayerActionType.Quit:
                _gameCompleted = false;
                return;
        }

        if (Player.IsDead) return;

        if (IsPlayerAtExit())
        {
            if (IsLastLevel())
            {
                _gameCompleted = true;
                return;
            }
            AdvanceToNextLevelInternal();
            return;
        }

        ProcessEnemiesTurn(skipEnemy: enemyEngagedInCombat);
    }

    private void HandlePlayerMove(int targetX, int targetY, out Enemy? engagedEnemy)
    {
        engagedEnemy = null;

        if (!_currentLevel.IsWalkable(targetX, targetY))
            return;

        var enemy = _currentLevel.GetEntitiesAt(targetX, targetY).OfType<Enemy>().FirstOrDefault();
        if (enemy != null)
        {
            engagedEnemy = enemy;
            var result = ProcessCombat(enemy);

            if (result == CombatResult.EnemyDefeated)
                MovePlayer(targetX, targetY);

            return;
        }

        MovePlayer(targetX, targetY);
    }
}


