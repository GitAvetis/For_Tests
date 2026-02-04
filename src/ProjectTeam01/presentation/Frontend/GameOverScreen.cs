using Mindmagma.Curses;

namespace ProjectTeam01.presentation.Frontend
{
    public static class GameOverScreen
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
        internal static void RenderGameOverScreen(nint stdscr, GameController controller)
        {
            NCurses.TimeOut(-1);

            NCurses.GetMaxYX(stdscr, out int _windowHeight, out int _windowWidth);
            NCurses.Clear();
            NCurses.AttributeOn(NCurses.ColorPair(1));
            int y = 0;
            int x = 0;
            for (int i = 0; i < helloScreen.Length; i++)
            {
                y = _windowHeight / 2 + i - helloScreen.Length;
                x = _windowWidth / 2 - helloScreen[i].Length / 2;
                NCursesMethods.Print(helloScreen[i],y, x);
            }

            string resultMessage = controller.Session.IsGameOver() 
            ? "GAME OVER - You died!" 
            : "VICTORY - You completed the game!";
        
            string instruction = "Press any key to return to main menu";
            
            int msgY = y / 2 + helloScreen.Length + 4;
            int msgX = _windowWidth / 2 - resultMessage.Length / 2;
            int instrY = msgY + 2;
            int instrX = _windowWidth / 2 - instruction.Length / 2;
            
            NCursesMethods.Print(resultMessage,msgY,Math.Max(0, msgX));
            NCursesMethods.Print(instruction,instrY,Math.Max(0, instrX));

            NCurses.AttributeOff(NCurses.ColorPair(1));
            NCurses.GetChar();
            NCurses.Refresh();
            NCurses.TimeOut(50);

        }

    }
    
}

