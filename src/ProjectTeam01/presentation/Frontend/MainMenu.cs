using Mindmagma.Curses;
using ProjectTeam01.presentation.Controllers;

namespace ProjectTeam01.presentation.Frontend
{
    public class MainMenu
    {
        private List<string> _menu { get; set; } = new();
        private readonly nint _stdscr;
        private MenuAction _menuAction;
        public bool GameOver { get; set; } = false;
        IGameSession? game = null;
        public MainMenu(nint stdscr, List<string> menu)
        {
            _menu = menu;
            _stdscr = stdscr;
        }

        public void ShowMenuAndGetChoice()
        {
            bool choiceMade = false;
            _menuAction = MenuAction.None;

            while (!choiceMade)
            {
                RenderMenu();
                int key = NCurses.GetChar();

                if (key >= '1' && key <= '4') // Выбор цифрой
                {
                    int index = key - '1';
                    if (index < _menu.Count)
                    {
                        _menuAction = ProcessChoice(index);
                        choiceMade = true;
                    }
                }
                else if (key == 'q' || key == '\x1b') // Выход
                {
                    _menuAction = MenuAction.Exit;
                    choiceMade = true;
                }
            }

        }

        private MenuAction ProcessChoice(int choiceIndex)
        {
            switch (choiceIndex)
            {
                case 0:
                    return MenuAction.NewGame;
                case 1:
                    return MenuAction.LoadGame;
                case 2:
                    return MenuAction.Scoreboard;
                case 3:
                    return MenuAction.Exit;
                default:
                    return MenuAction.None;
            }
        }

        public void RenderMenu()
        {
            NCurses.GetMaxYX(_stdscr, out int maxY, out int maxX);
            NCurses.Clear();

            int startY = maxY / 2 - _menu.Count / 2;

            for (int i = 0; i < _menu.Count; i++)
            {
                string menuItem = _menu[i];
                int x = maxX / 2 - menuItem.Length / 2;
                int y = startY + i;

                if (x >= 0 && x < maxX && y >= 0 && y < maxY)
                {
                    NCurses.Move(y, x);
                    NCurses.AddString(menuItem);
                }
            }
            string instructions = "Press 1-4 to select, ESC to exit";

            int instX = maxX / 2 - instructions.Length / 2;
            int instY = startY + _menu.Count + 2;

            if (instX >= 0 && instX < maxX && instY >= 0 && instY < maxY)
            {
                NCurses.Move(instY, instX);
                NCurses.AddString(instructions);
            }

            NCurses.Refresh();
        }


        private IGameSession? Mode()
        {
            switch (_menuAction)
            {
                case MenuAction.NewGame:
                    return new StartNewGame();
                case MenuAction.LoadGame:
                    return new StartFromSave("game_save.json");
                case MenuAction.Scoreboard:
                    return null;
                default:
                    return null;
            }
        }
        public bool GameCyqle()
        {
            ShowMenuAndGetChoice();
            game = Mode();
            bool isRunning = true;
            if (game == null)
            {
                if (_menuAction == MenuAction.Scoreboard)
                {
                    PrintScoreboard.ShowScoreboard(_stdscr);
                    return true;
                }
                else
                    return false;
            }

            while (isRunning)
            {
                int input = NCurses.GetChar();
                isRunning = game.IsGameRunning(input);
                game.RenderGameScreen(_stdscr);
            }
            return true;
        }

    }

    // Действия меню
    public enum MenuAction
    {
        None,
        NewGame,
        LoadGame,
        Scoreboard,
        Exit
    }

}

