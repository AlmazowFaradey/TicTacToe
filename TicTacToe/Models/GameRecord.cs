namespace TicTacToe.Models
{
    public class GameRecord
    {
        public string Winner { get; set; } = "";
        public string Loser { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
        public int WinnerScore { get; set; }
        public int LoserScore { get; set; }
    }
}
