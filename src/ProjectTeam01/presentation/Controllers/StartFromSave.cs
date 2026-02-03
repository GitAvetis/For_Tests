using ProjectTeam01.datalayer;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Session;
using ProjectTeam01.presentation.Frontend;

namespace ProjectTeam01.presentation.Controllers
{
    internal class StartFromSave : IGameSession
    {
        private GameController _controller;
        private GameSession _game;
        private char[,] _map;
        public StartFromSave(string save)
        {
            _game = GameDataService.LoadGame(save);
            _controller = new GameController(_game);
            _map = new char[GenerationConstants.MapHeight, GenerationConstants.MapWidth];
        }

        public bool IsGameRunning(int key)
        {
            return  _controller.HandleInput((char)key);
        }

        public void RenderGameScreen(nint stdscr, MainMenu mainMenu)
        {

            // Инициализируем карту пробелами
            for (int y = 0; y < GenerationConstants.MapHeight; y++)
            {
                for (int x = 0; x < GenerationConstants.MapWidth; x++)
                {
                    _map[y, x] = ' ';
                }
            } 

            GameStateRenderer.RenderHandler(_controller.GetGameStateViewModel(), stdscr, _controller, _map, mainMenu);
        }
            
    }
    
}

