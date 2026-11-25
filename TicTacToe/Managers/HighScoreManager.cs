using System.Text.Json;
using TicTacToe.Models;

namespace TicTacToe.Managers
{
    public class HighScoreManager
    {
        private const string FilePath = "highscores.json";
        private readonly List<GameRecord> _records = new();

        public HighScoreManager() { Load(); }

        private void Load()
        {
            if (!File.Exists(FilePath)) return;
            try
            {
                var loaded = JsonSerializer.Deserialize<List<GameRecord>>(File.ReadAllText(FilePath));
                if (loaded is not null) _records.AddRange(loaded);
            }
            catch { }
        }

        public void Save() => File.WriteAllText(FilePath,
            JsonSerializer.Serialize(_records, new JsonSerializerOptions { WriteIndented = true }));

        public void AddRecord(GameRecord r) { _records.Add(r); Save(); }

        public IReadOnlyList<GameRecord> GetRecords() =>
            _records.OrderByDescending(r => r.WinnerScore).ToList();
    }
}
