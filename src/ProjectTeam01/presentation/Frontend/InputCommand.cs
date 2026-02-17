namespace ProjectTeam01.presentation.Frontend
{
    public enum InputCommandType
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        WeaponMenu,
        FoodMenu,
        ElixirMenu,
        ScrollMenu,
        Quit
    }

    public class InputCommand
    {
        public InputCommandType Type { get; }

        public InputCommand(InputCommandType type)
        {
            Type = type;
        }
    }
}
