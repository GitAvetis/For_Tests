using Mindmagma.Curses;

namespace ProjectTeam01.presentation.Frontend
{
    public class NCursesMethods
    {
        public static void Init(nint stdscr)
        {
            NCurses.NoEcho();
            NCurses.CBreak();
            NCurses.Keypad(stdscr, true);

            NCurses.TimeOut(50);
        }

        public static void Shutdown()
        {
            NCurses.EndWin();
        }
    }
}
