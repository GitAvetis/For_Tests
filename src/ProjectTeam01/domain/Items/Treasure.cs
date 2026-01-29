namespace ProjectTeam01.domain.Items
{
    internal class Treasure : Item
    {
        public Treasure(int posX, int posY, int lvl) : base(ItemType.Treasure, posX, posY)
        {
            Level = lvl;
        }
        public int Price { get; set; }
        public int Level { get; } 
    }
}
