using TicTacToe.Enums;
using TicTacToe.InputActions;
using TicTacToe.Managers;
using TicTacToe.Models;

namespace TicTacToe
{
    public class Game
    {
        private GameState _state = GameState.MainMenu;
        private readonly Board _board = new();
        private readonly Player _playerX = new("Игрок X", Cell.X);
        private readonly Player _playerO = new("Игрок O", Cell.O);
        private Player _currentPlayer;
        private readonly HighScoreManager _highScores = new();
        private int _cursorRow;
        private int _cursorCol;

        public Game() { _currentPlayer = _playerX; }

        public void Run()
        {
            while (_state != GameState.Exiting)
            {
                switch (_state)
                {
                    case GameState.MainMenu: ShowMainMenu(); break;
                    case GameState.Playing: PlayTurn(); break;
                    case GameState.Paused: ShowPauseMenu(); break;
                    case GameState.GameOver: ShowGameOver(); break;
                    case GameState.ViewingHighScores: ShowHighScores(); break;
                }
            }
        }

        private void ShowMainMenu()
        {
            int choice = MenuSelect("=== КРЕСТИКИ-НОЛИКИ (.NET 10) ===",
                ["Новая игра", "Рекорды", "Выход"]);

            switch (choice)
            {
                case 0: ResetGame(); _state = GameState.Playing; break;
                case 1: _state = GameState.ViewingHighScores; break;
                case 2: _state = GameState.Exiting; break;
            }
        }

        private void ShowPauseMenu()
        {
            int choice = MenuSelect("=== ПАУЗА ===", ["Продолжить", "Выйти в главное меню"]);
            _state = choice == 0 ? GameState.Playing : GameState.MainMenu;
        }

        private void ShowGameOver()
        {
            int choice = MenuSelect("=== ПАРТИЯ ЗАВЕРШЕНА ===", ["Сыграть ещё", "Главное меню"]);
            if (choice == 0) { ResetGame(); _state = GameState.Playing; }
            else { _state = GameState.MainMenu; }
        }

        private void PlayTurn()
        {
            string status = "";
            while (true)
            {
                DrawPlayScreen(status);
                status = "";

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        _cursorRow = (_cursorRow - 1 + 3) % 3; break;
                    case ConsoleKey.DownArrow:
                        _cursorRow = (_cursorRow + 1) % 3; break;
                    case ConsoleKey.LeftArrow:
                        _cursorCol = (_cursorCol - 1 + 3) % 3; break;
                    case ConsoleKey.RightArrow:
                        _cursorCol = (_cursorCol + 1) % 3; break;
                    case ConsoleKey.Escape:
                        _state = GameState.Paused;
                        return;
                    case ConsoleKey.Enter:
                        if (!_board.IsEmpty(_cursorRow, _cursorCol))
                        {
                            status = "Эта клетка занята!";
                            break;
                        }
                        int r = _cursorRow, c = _cursorCol;
                        _cursorRow = 0; _cursorCol = 0;
                        MakeMove(r, c);
                        return;
                }
            }
        }

        private void MakeMove(int row, int col)
        {
            _board.Place(row, col, _currentPlayer.Mark);

            var winner = _board.CheckWinner();
            if (winner is not null)
            {
                EndGame(winner == Cell.X ? _playerX : _playerO,
                        winner == Cell.X ? _playerO : _playerX);
                return;
            }
            if (_board.IsFull()) { EndGame(null, null); return; }

            _currentPlayer = _currentPlayer == _playerX ? _playerO : _playerX;
        }

        private void DrawPlayScreen(string status)
        {
            Console.Clear();
            Console.ForegroundColor = _currentPlayer.Mark == Cell.X ? ConsoleColor.Red : ConsoleColor.Blue;
            Console.WriteLine($"=== Ход: {_currentPlayer.Name} ({_currentPlayer.Mark}) ===");
            Console.ResetColor();
            Console.WriteLine();

            DrawBoard(highlightCursor: true);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Стрелки — перемещение, Enter — поставить, Esc — пауза");
            Console.ResetColor();

            if (!string.IsNullOrEmpty(status))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠ {status}");
                Console.ResetColor();
            }
        }

        private void DrawBoard(bool highlightCursor = false)
        {
            Console.WriteLine("      0   1   2");
            Console.WriteLine("    ┌───┬───┬───┐");
            for (int r = 0; r < 3; r++)
            {
                Console.Write($"  {r} │");
                for (int c = 0; c < 3; c++)
                {
                    var symbol = _board.Cells[r, c] switch
                    {
                        Cell.X => "X",
                        Cell.O => "O",
                        _ => " "
                    };
                    bool isCursor = highlightCursor && r == _cursorRow && c == _cursorCol;

                    if (isCursor)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (_board.Cells[r, c] == Cell.X)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (_board.Cells[r, c] == Cell.O)
                        Console.ForegroundColor = ConsoleColor.Blue;

                    Console.Write($" {symbol} ");
                    Console.ResetColor();
                    Console.Write("│");
                }
                Console.WriteLine();
                if (r < 2) Console.WriteLine("    ├───┼───┼───┤");
            }
            Console.WriteLine("    └───┴───┴───┘");
            Console.WriteLine();
        }

        private void EndGame(Player? winner, Player? loser)
        {
            Console.Clear();
            DrawBoard();

            if (winner is not null && loser is not null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"🏆 {winner.Name} победил!");
                Console.ResetColor();
                _highScores.AddRecord(new GameRecord
                {
                    Winner = winner.Name,
                    Loser = loser.Name,
                    WinnerScore = 10,
                    LoserScore = 0
                });
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("🤝 Ничья!");
                Console.ResetColor();
                _highScores.AddRecord(new GameRecord
                {
                    Winner = "Ничья",
                    Loser = "Ничья",
                    WinnerScore = 5,
                    LoserScore = 5
                });
            }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey(true);
            _state = GameState.GameOver;
        }

        private void ShowHighScores()
        {
            Console.Clear();
            Console.WriteLine("=== РЕКОРДЫ ===\n");
            var records = _highScores.GetRecords();
            if (records.Count == 0)
            {
                Console.WriteLine("Пока нет записей.");
            }
            else
            {
                Console.WriteLine($"{"Победитель",-15} {"Проигравший",-15} {"Очки",6} {"Дата",-20}");
                Console.WriteLine(new string('─', 60));
                foreach (var r in records.Take(10))
                    Console.WriteLine($"{r.Winner,-15} {r.Loser,-15} {r.WinnerScore,6} {r.Date:g}");
            }
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey(true);
            _state = GameState.MainMenu;
        }

        private void ResetGame()
        {
            _board.Reset();
            _currentPlayer = _playerX;
            _cursorRow = 0;
            _cursorCol = 0;
        }

        private static int MenuSelect(string title, string[] options)
        {
            int selected = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(title);
                Console.WriteLine();
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"  ► {options[i]}");
                        Console.ResetColor();
                    }
                    else Console.WriteLine($"    {options[i]}");
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("↑/↓ — выбор, Enter — подтвердить");
                Console.ResetColor();

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        selected = (selected - 1 + options.Length) % options.Length; break;
                    case ConsoleKey.DownArrow:
                        selected = (selected + 1) % options.Length; break;
                    case ConsoleKey.Enter:
                        return selected;
                }
            }
        }
    }
}
