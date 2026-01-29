using ProjectTeam01.domain.Items;

namespace ProjectTeam01.domain;

/// Действие игрока
internal class PlayerAction
{
    public PlayerActionType Type { get; }
    public int TargetX { get; }
    public int TargetY { get; }
    public Item? SelectedItem { get; }

    private PlayerAction(PlayerActionType type, int targetX = 0, int targetY = 0, Item? selectedItem = null)
    {
        Type = type;
        TargetX = targetX;
        TargetY = targetY;
        SelectedItem = selectedItem;
    }

    public static PlayerAction CreateMove(int x, int y) => new(PlayerActionType.Move, x, y);
    public static PlayerAction CreateUseItem(Item item) => new(PlayerActionType.UseItem, selectedItem: item);
    public static PlayerAction CreateEquipWeapon(Weapon weapon) => new(PlayerActionType.EquipWeapon, selectedItem: weapon);
    public static PlayerAction CreateUnequipWeapon() => new(PlayerActionType.UnequipWeapon);
    public static PlayerAction CreateDropItem(Item item) => new(PlayerActionType.DropItem, selectedItem: item);
    public static PlayerAction CreateQuit() => new(PlayerActionType.Quit);
}

/// Тип действия игрока
internal enum PlayerActionType
{
    Move,
    UseItem,
    EquipWeapon,
    UnequipWeapon,
    DropItem,
    Quit
}

