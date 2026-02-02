using Mindmagma.Curses;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Session;
using ProjectTeam01.presentation.Frontend;
using ProjectTeam01.presentation;
namespace ProjectTeam01
{
    internal class Program
    {
        static readonly nint stdscr = NCurses.InitScreen();
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
            // Создаем новую игру через GameInitializer (domain слой)
            var gameSession = GameInitializer.CreateNewGame(levelNumber: 1);
            var controller = new GameController(gameSession);
            bool running = true;
            NCursesMethods.ActivateColorSystem(stdscr);
            MainMenu.RenderMainMenu(stdscr);
            while (true)
            {
                int key = NCurses.GetChar();
                if ((int)key == '\n'  || key == 'q') // Enter
                {
                    break;
                }
            }
            // Создаем карту
            char[,] map = new char[GenerationConstants.MapHeight, GenerationConstants.MapWidth];
            while (running)
            {
            // Инициализируем карту пробелами
            for (int y = 0; y < GenerationConstants.MapHeight; y++)
            {
                for (int x = 0; x < GenerationConstants.MapWidth; x++)
                {
                    map[y, x] = ' ';
                }
            }  
                var viewModel = controller.GetGameStateViewModel();

                // Отрисовка
                GameStateRenderer.RenderHandler(viewModel, stdscr, controller,map);

                // Получаем ввод
                int key = NCurses.GetChar(); // ждет нажатия клавиши

                // Обработка ввода
                running = controller.HandleInput((char)key);
            }

        }

    }
}
