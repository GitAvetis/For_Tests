using Mindmagma.Curses;

namespace ProjectTeam01.presentation.Frontend
{
    public static class HelloScreen
    {
        private static readonly string[] helloScreen =
        {
            "     ______ ______ __  __ ______ ______    ",
            "    / __  // __  // / / // ____// ____/    ",
            "   / /_/ // / / // / / // /___ / /___      ",
            "  / _  _// / / // / / // //_ //  ___/      ",
            " / // / / /_/ // /_/ // /__/// /___        ",
            "/_//_/ /_____//_____//_____//_____/        ",
            "               __     ____  __   ___ ______",
            "              / /    /_ _/ / / _/  // ____/",
            "             / /      //  / /_/ __// /___  ",
            "            / /      //  /  _  /_ /  ___/  ",
            "           / /___  _//_ / / /_  // /___    ",
            "          /_____/ /___//_/   /_//_____/    "
        };
        public static void RenderHelloScreen(nint stdscr)
        {
            NCurses.GetMaxYX(stdscr, out int _windowHeight, out int _windowWidth);
            NCurses.Clear();
            NCurses.AttributeOn(NCurses.ColorPair(1));

            // for (int i = 1; i < _windowHeight - 1; i++)
            // {
            //     for (int j = 1; j < _windowWidth - 1; j++)
            //     {
            //         NCursesMethods.Print(".",i,j);
            //     }
            // }
            int x = 0;
            int y = 0;
            for (int i = 0; i < helloScreen.Length; i++)
            {
                y = _windowHeight / 2 + i - helloScreen.Length;
                x = _windowWidth / 2 - helloScreen[i].Length / 2;
                NCursesMethods.Print(helloScreen[i], y, x);
            }

            string instruction = "Press ENTER or Q to return to main menu";

            NCursesMethods.Print(instruction, y + 3, x + 1);

            NCurses.AttributeOff(NCurses.ColorPair(1));

            NCurses.Refresh();
        }

    }

}

