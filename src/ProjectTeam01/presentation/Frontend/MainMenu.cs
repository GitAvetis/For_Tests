using Mindmagma.Curses;
using ProjectTeam01.domain;
using ProjectTeam01.presentation.Controllers;

namespace ProjectTeam01.presentation.Frontend
{
    public class MainMenu
    {
        private List<string> _menu{get;set;} = new();
        private readonly int _selectedIndex = 0;
        private readonly nint _stdscr;
        public MainMenu (nint stdscr, List<string> menu)
        {
            _menu = menu;
            _stdscr = stdscr;
        }
          
        // Основной метод для показа меню и получения выбора
        public MenuResult ShowMenuAndGetChoice(nint stdscr)
        {
            bool choiceMade = false;
            MenuResult result = new MenuResult { Choice = -1, Action = MenuAction.None };
            
            while (!choiceMade)
            {
                RenderMenu();
                int key = NCurses.GetChar();
                
                // Обработка выбора
                if (key == '\n') // Enter
                {
                    result = ProcessChoice(_selectedIndex);
                    choiceMade = true;
                }
                else if (key >= '1' && key <= '4') // Выбор цифрой
                {
                    int index = key - '1';
                    if (index < _menu.Count)
                    {
                        result = ProcessChoice(index);
                        choiceMade = true;
                    }
                }
                else if (key == 'q' || key == '\x1b') // Выход
                {
                    result = new MenuResult { Choice = 3, Action = MenuAction.Exit };
                    choiceMade = true;
                }
            }
            
            return result;
        }
        
        private MenuResult ProcessChoice(int choiceIndex)
        {
            switch (choiceIndex)
            {
                case 0:
                    return new MenuResult { Choice = 0, Action = MenuAction.NewGame };
                case 1:
                    return new MenuResult { Choice = 1, Action = MenuAction.LoadGame };
                case 2: 
                    return new MenuResult { Choice = 2, Action = MenuAction.Scoreboard };
                case 3:
                    return new MenuResult { Choice = 3, Action = MenuAction.Exit };
                default:
                    return new MenuResult { Choice = -1, Action = MenuAction.None };
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
        
        public IGameSession? CreateGameSession (MenuResult result, string savePath = "game_save.json")
        {
            switch (result.Action)
            {
                case MenuAction.NewGame: 
                    return new StartNewGame();
                    
                case MenuAction.LoadGame:
                    return new StartFromSave(savePath);
                    
                default:
                    return null;
            }
        }
    }
    
    // Результат выбора в меню
    public struct MenuResult
    {
        public int Choice { get; set; }
        public MenuAction Action { get; set; }
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

