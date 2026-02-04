using Mindmagma.Curses;
using ProjectTeam01.presentation.Frontend;
namespace ProjectTeam01
{
    internal class Program
    {
        static readonly nint stdscr = NCurses.InitScreen();
        static readonly List<string> menu = new() {
            "1. Sart new game",
            "2. Load game",
            "3. Score board",
            "4. Exit"
            };

        static void Main(string[] args)
        {
            try
            {
                NCursesMethods.Init(stdscr);
                InitializeGame();
            }
            finally
            {
                NCursesMethods.Shutdown();
            }
        }
        static void InitializeGame()
        {
            NCursesMethods.ActivateColorSystem(stdscr);
            HelloScreen.RenderHelloScreen(stdscr);
            WaitForAnyKey();

            MainMenu mainMenu = new(stdscr, menu);
            bool isRunning = true;
            while (isRunning)
            {
                isRunning = mainMenu.GameCyqle();
            }

        }

        private static void WaitForAnyKey()
        {
            while (true)
            {
                int key = NCurses.GetChar();
                if (key == '\n' || key == 'q') // Enter
                {
                    break;
                }
            }
            NCurses.FlushInputBuffer();// сбросил энтер из инпута
        }

    }
}
