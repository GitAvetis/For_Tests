namespace ProjectTeam01.presentation.Frontend
{
    public enum InputMode
    {
        Normal,
        WeaponMenu,
        FoodMenu,
        ElixirMenu,
        ScrollMenu
    }
    public static class InputHandler
    {
        public static InputCommand? Read(char key)
        {
            if (key == 'w' || key == 'W')
                return new (InputCommandType.MoveUp);
            if (key == 's' || key == 'S')
                return new (InputCommandType.MoveDown);
            if (key == 'a' || key == 'A')
                return new (InputCommandType.MoveLeft);
            if (key == 'd' || key == 'D')
                return new (InputCommandType.MoveRight);

            if (key == 'h' || key == 'H')
                return new (InputCommandType.WeaponMenu);
            if (key == 'j' || key == 'J')
                return new (InputCommandType.FoodMenu);
            if (key == 'k' || key == 'K')
                return new (InputCommandType.ElixirMenu);
            if (key == 'e' || key == 'E')
                return new (InputCommandType.ScrollMenu);

            if (key == 'q' || key == 'Q' || key == '\x1b')
                return new (InputCommandType.Quit);

            return null;
        }
    }
}

