using Mindmagma.Curses;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Session;
using ProjectTeam01.presentation.Frontend;
using ProjectTeam01.presentation;
using ProjectTeam01.presentation.Controllers;
namespace ProjectTeam01
{
    internal class Program
    {
        static readonly nint stdscr = NCurses.InitScreen();
        static readonly List<string> menu = new () {
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

            MainMenu mainMenu = new(stdscr,menu);
            StartFromSave game = new("game_save.json");
            bool isRunning = true;
            while (isRunning)
            {   
                int input = NCurses.GetChar();
                if(input == 3)
                    break;
                else
                    mainMenu.ShowMenuAndGetChoice(input);
                // isRunning = game.IsGameRunning(input);
                // game.RenderGameScreen(stdscr,mainMenu);
            }
            // StartNewGame game = new();
            // bool isRunning = true;
            // while (isRunning)
            // {   
            //     int input = NCurses.GetChar();
            //     isRunning = game.IsGameRunning(input);
            //     game.RenderGameScreen(stdscr,mainMenu);
            // }
        }
        // static void InitializeGame()
        // {
        //     // Создаем новую игру через GameInitializer (domain слой)
        //     var gameSession = GameInitializer.CreateNewGame(levelNumber: 1);
        //     var controller = new GameController(gameSession);
        //     bool running = true;
        //     MainMenu mainMenu = new(menu);
        //     NCursesMethods.ActivateColorSystem(stdscr);
        //     HelloScreen.RenderHelloScreen(stdscr);
        //     while (true)
        //     {
        //         int key = NCurses.GetChar();
        //         if (key == '\n'  || key == 'q') // Enter
        //         {
        //             break;
        //         }
        //     }
        //     NCurses.FlushInputBuffer();// сбросил энтер из инпута

        //     // Создаем карту
        //     char[,] map = new char[GenerationConstants.MapHeight, GenerationConstants.MapWidth];
        //     while (running)
        //     {
        //     // Инициализируем карту пробелами
        //     for (int y = 0; y < GenerationConstants.MapHeight; y++)
        //     {
        //         for (int x = 0; x < GenerationConstants.MapWidth; x++)
        //         {
        //             map[y, x] = ' ';
        //         }
        //     }  
        //         var viewModel = controller.GetGameStateViewModel();

        //         // Отрисовка
        //         GameStateRenderer.RenderHandler(viewModel, stdscr, controller,map,mainMenu);

        //         // Получаем ввод
        //         int key = NCurses.GetChar(); // ждет нажатия клавиши

        //         // Обработка ввода
        //         running = controller.HandleInput((char)key);
        //     }

        // }

        private static void WaitForAnyKey()
        {
            while (true)
            {
                int key = NCurses.GetChar();
                if (key == '\n'  || key == 'q') // Enter
                {
                    break;
                }
            }
        }

    }
}
