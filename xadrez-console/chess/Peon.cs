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

        private bool existEnemy(Position pos)
        {
            Piece p = board.piece(pos);
            return p != null && p.color != color;
        }

        private bool freePosition(Position pos)
        {
            return board.piece(pos) == null;
        }

        public override bool[,] possibleMoves()
        {
            bool[,] mat = new bool[board.lines, board.columns];
            Position pos = new Position(0, 0);

            if (color == Color.White)
            {
                pos.setValues(position.line - 1, position.column);
                if (board.validPosition(pos) && freePosition(pos))
                {
                    mat[pos.line, pos.column] = true;
                }

                pos.setValues(position.line - 2, position.column);
                if (board.validPosition(pos) && freePosition(pos) && numberOfMoves == 0)
                {
                    mat[pos.line, pos.column] = true;
                }

                pos.setValues(position.line - 1, position.column - 1);
                if (board.validPosition(pos) && existEnemy(pos))
                {
                    mat[pos.line, pos.column] = true;
                }

                pos.setValues(position.line - 1, position.column + 1);
                if (board.validPosition(pos) && existEnemy(pos))
                {
                    mat[pos.line, pos.column] = true;
                }
            }
            else
            {
                pos.setValues(position.line + 1, position.column);
                if (board.validPosition(pos) && freePosition(pos))
                {
                    mat[pos.line, pos.column] = true;
                }

                pos.setValues(position.line + 2, position.column);
                if (board.validPosition(pos) && freePosition(pos) && numberOfMoves == 0)
                {
                    mat[pos.line, pos.column] = true;
                }

                pos.setValues(position.line + 1, position.column - 1);
                if (board.validPosition(pos) && existEnemy(pos))
                {
                    mat[pos.line, pos.column] = true;
                }

                pos.setValues(position.line + 1, position.column + 1);
                if (board.validPosition(pos) && existEnemy(pos))
                {
                    mat[pos.line, pos.column] = true;
                }
            }

            return mat;
        }
    }
}
