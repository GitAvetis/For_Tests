using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.datalayer.Mappers
{
    internal class ItemMapper
    {
        public static ItemSave ToSave(Item item)
        {
            ItemSave save = new()
            {
                Type = item.Type,
                PosX = item.Position.X,
                PosY = item.Position.Y
            };

            switch (item)
            {
                case Food food:
                    save.HealthValue = food.HealthValue;
                    break;
                case Elixir elixir:
                    save.ElixirType = elixir.ElixirType;
                    break;
                case Treasure treasure:
                    save.Price = treasure.Price;
                    break;
                case Weapon weapon:
                    save.WeaponType = weapon.WeaponType;
                    break;
                case Scroll scroll:
                    save.ScrollType = scroll.ScrollType;
                    break;
            }

            return save;
        }
        public static Item FromSave(ItemSave save)
        {
            switch (save.Type)
            {
                case ItemType.Treasure:
                    return new Treasure(save.PosX, save.PosY, save.GameLevel) { Price = save.Price!.Value };
                case ItemType.Food:
                    return new Food(save.HealthValue ?? 10, save.PosX, save.PosY);
                case ItemType.Elixir:
                    return new Elixir(save.ElixirType!.Value, save.PosX, save.PosY);
                case ItemType.Scroll:
                    return new Scroll(save.ScrollType!.Value, save.PosX, save.PosY);
                case ItemType.Weapon:
                    return new Weapon(save.WeaponType!.Value, save.PosX, save.PosY);
                default:
                    throw new ArgumentOutOfRangeException($"Unknown item type: {save.Type}");
            }
        }
    }
}
