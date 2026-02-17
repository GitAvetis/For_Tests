namespace ProjectTeam01.domain.Items
{
    /// Тип предмета для использования в ViewModels и других публичных API
    public enum ItemType
    {
        Treasure,
        Food,
        Elixir,
        Scroll,
        Weapon
    }

    internal abstract class Item : GameObject
    {
        public ItemType Type { get; }

        protected Item(ItemType type, int posX, int posY) : base(posX, posY)
        {
            Type = type;
        }
    }
}
