namespace ProjectTeam01.datalayer.Models;

/// Модель для таблицы лидеров - содержит список всех попыток прохождения
internal class ScoreboardSave
{
    /// Список статистик всех попыток прохождения
    public List<GameStatisticsSave> SessionStats { get; set; } = new();
}

