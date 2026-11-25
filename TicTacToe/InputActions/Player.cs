using TicTacToe.Enums;

namespace TicTacToe.InputActions
{
    public class Player(string name, Cell mark)
    {
        public string Name { get; } = name;
        public Cell Mark { get; } = mark;
    }
}
