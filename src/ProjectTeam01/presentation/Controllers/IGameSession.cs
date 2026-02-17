namespace ProjectTeam01.presentation.Controllers
{
    public interface IGameSession
    {
        bool IsGameRunning(int key);
        void RenderGameScreen(nint stdscr);
    }
}