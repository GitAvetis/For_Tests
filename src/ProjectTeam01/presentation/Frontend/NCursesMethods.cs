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
            NCurses.SetCursor(0);

            NCurses.TimeOut(50);
        }

        public static void Shutdown()
        {
            NCurses.EndWin();
        }

        public static void Print(string text, int y, int x)
        {
            // NCurses.AttributeOn(NCurses.ColorPair(1));
            NCurses.Move(y, x);
            NCurses.AddString(text);
            // NCurses.AttributeOff(NCurses.ColorPair(1));

        }
        public static void SetBackgroundColor(short backgroundColor, nint stdscr)
        {
            // Устанавливаем фон для всего экрана
            NCurses.WindowBackground(stdscr, NCurses.ColorPair(backgroundColor));
            NCurses.Clear();
        }
        public static void ActivateColorSystem(nint stdscr)
        {
            NCurses.StartColor();
            NCurses.UseDefaultColors();
            if (NCurses.CanChangeColor())
            {
                NCurses.InitColor(UiColors.White, 1000, 1000, 1000);
                NCurses.InitColor(UiColors.Yellow, 1000, 1000, 0);
                NCurses.InitColor(UiColors.Red, 1000, 0, 0);
                NCurses.InitColor(UiColors.Green, 0, 1000, 0);
                NCurses.InitColor(UiColors.Blue, 0, 0, 1000);
                NCurses.InitColor(UiColors.Black, 0, 0, 0);

                NCurses.InitPair(1, UiColors.White, UiColors.Black);
                NCurses.InitPair(4, UiColors.Green, UiColors.Black);
                NCurses.InitPair(2, UiColors.Yellow, UiColors.Black);
                NCurses.InitPair(3, UiColors.Red, UiColors.Black);
                NCurses.InitPair(5, UiColors.Blue, UiColors.Black);
                NCurses.InitPair(6, UiColors.Black, UiColors.Black);
            }
            SetBackgroundColor(1, stdscr);
        }
    }
}
