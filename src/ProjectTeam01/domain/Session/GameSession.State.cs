namespace ProjectTeam01.domain.Session;
// отвечает за получение состояния
internal partial class GameSession
{
    /// Получить состояние игры для отображения
    public GameState GetGameState()
    {
        return new GameState
        {
            PlayerPosition = Player.Position,
            Enemies = _currentLevel.GetEnemies(),
            Items = _currentLevel.GetItems(),
            LevelGeometry = _currentLevel,
            PlayerHealth = Player.ActualHp,
            PlayerMaxHealth = Player.MaxHp,
            PlayerAgility = Player.Agility,
            PlayerStrength = Player.Strength,
            PlayerIsSleep = Player.IsHeroSleep,
            CurrentLevelNumber = _currentLevel.LevelNumber,
            // Инвентарь игрока
            PlayerWeapons = GetPlayerWeapons(),
            PlayerFood = GetPlayerFood(),
            PlayerElixirs = GetPlayerElixirs(),
            PlayerScrolls = GetPlayerScrolls()
        };
    }
}


