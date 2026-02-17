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
            var sortedScoreboard = scoreboard
            .OrderByDescending(s => s.TreasuresCollected).Take(5).ToList();
            int y = maxY / 2;

            for (int i = 0; i < sortedScoreboard.Count; i++)
            {
                string gold = sortedScoreboard[i].TreasuresCollected.ToString();
                string lvl = sortedScoreboard[i].MaxLevelReached.ToString();
                string message = $"{i + 1}. Gold collected {gold}.....Lvl {lvl}.";
                int x = maxX / 2 - message.Length / 2;
                NCursesMethods.Print(message,y + i, x);
            }

            NCurses.GetChar();
            NCurses.Refresh();
            NCurses.TimeOut(50);
        }

    }

}

