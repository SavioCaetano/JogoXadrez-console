using board;

namespace chess
{
    class Peon : Piece
    {
        public Peon(Board board, Color color) : base(board, color)
        {
        }

        public override string ToString()
        {
            return "P";
        }
    }
}
