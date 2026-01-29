namespace ProjectTeam01.datalayer.Models
{
    internal class GameSave
    {
        public HeroSave Hero { get; set; } = null!;
        public List<EnemySave> Enemies { get; set; } = new();
        public List<ItemSave> Items { get; set; } = new();
        public LevelSave Level { get; set; } = null!;
        public GameStatisticsSave Statistics { get; set; } = null!;
        public int GameLevel { get; set; }
    }
}
