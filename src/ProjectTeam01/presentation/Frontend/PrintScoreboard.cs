using Mindmagma.Curses;
using ProjectTeam01.datalayer;


namespace ProjectTeam01.presentation.Frontend
{
    internal class PrintScoreboard 
    {
        public static void ShowScoreboard(nint _stdscr)
        {
            NCurses.TimeOut(-1);
            NCurses.Clear();
            NCurses.GetMaxYX(_stdscr, out int maxY, out int maxX);

            var scoreboard = GameDataService.LoadScoreboard("scoreboard.json").SessionStats;
            var sortedScoreboard =scoreboard
            .OrderByDescending(s=>s.TreasuresCollected).ToList();      
            int y = maxY / 2;

            for (int i = 0; i < sortedScoreboard.Count; i++)
            {
                string message = sortedScoreboard[i].TreasuresCollected.ToString();
                int x = maxX / 2 - message.Length / 2;
                NCurses.Move(y+i, x);
                NCurses.AddString($"{i+1}. {message}");
            } 

            NCurses.GetChar();
            NCurses.Refresh();
            NCurses.TimeOut(50);
        }
            
    }
    
}

