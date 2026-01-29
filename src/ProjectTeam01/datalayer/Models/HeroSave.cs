namespace ProjectTeam01.datalayer.Models
{
    internal class HeroSave
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int CurrentHp { get; set; }
        public int BaseMaxHp { get; set; }
        public int BaseStrength { get; set; }
        public int BaseAgility { get; set; }
        public List<ActiveEffectSave> ActiveEffects { get; set; } = new();
        public ItemSave? EquippedWeapon { get; set; }
        public List<ItemSave> HeroBackpack { get; set; } = new();
        public bool IsHeroSleep { get; set; }
    }
}
