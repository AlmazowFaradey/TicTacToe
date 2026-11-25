using TicTacToe.Enums;

namespace TicTacToe.InputActions
{
    public class Board
    {
        public Cell[,] Cells { get; } = new Cell[3, 3];
        public bool IsEmpty(int r, int c) => Cells[r, c] == Cell.Empty;
        public void Place(int r, int c, Cell mark) => Cells[r, c] = mark;
        public bool IsFull() => Cells.Cast<Cell>().All(c => c != Cell.Empty);

        public Cell? CheckWinner()
        {
            for (int r = 0; r < 3; r++)
                if (Cells[r, 0] != Cell.Empty && Cells[r, 0] == Cells[r, 1] && Cells[r, 1] == Cells[r, 2])
                    return Cells[r, 0];
            for (int c = 0; c < 3; c++)
                if (Cells[0, c] != Cell.Empty && Cells[0, c] == Cells[1, c] && Cells[1, c] == Cells[2, c])
                    return Cells[0, c];
            if (Cells[0, 0] != Cell.Empty && Cells[0, 0] == Cells[1, 1] && Cells[1, 1] == Cells[2, 2])
                return Cells[0, 0];
            if (Cells[0, 2] != Cell.Empty && Cells[0, 2] == Cells[1, 1] && Cells[1, 1] == Cells[2, 0])
                return Cells[0, 2];
            return null;
        }

        public void Reset() => Array.Clear(Cells);
    }
}
