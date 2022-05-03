using System.Collections.Generic;
using board;

namespace chess
{
    class ChessGame
    {
        public Board board { get; private set; }
        public int turn { get; private set; }
        public Color currentPlayer { get; private set; }
        public bool finished { get; private set; }
        private HashSet<Piece> pieces;
        private HashSet<Piece> captured;
        public bool check { get; private set; }
        public Piece vulnerableEnPassant { get; private set; }

        public ChessGame()
        {
            board = new Board(8, 8);
            turn = 1;
            currentPlayer = Color.White;
            finished = false;
            check = false;
            vulnerableEnPassant = null;
            pieces = new HashSet<Piece>();
            captured = new HashSet<Piece>();
            placePieces();
        }

        public Piece performMoviment(Position origin, Position destiny)
        {
            Piece p = board.removePiece(origin);
            p.incrementNumberOfMoves();
            Piece capturedPiece = board.removePiece(destiny);
            board.putPiece(p, destiny);
            if (capturedPiece != null)
            {
                captured.Add(capturedPiece);
            }

            // #jogadaespecial roque pequeno
            if (p is King && destiny.column == origin.column + 2)
            {
                Position originT = new Position(origin.line, origin.column + 3);
                Position destinyT = new Position(origin.line, origin.column + 1);
                Piece T = board.removePiece(originT);
                T.incrementNumberOfMoves();
                board.putPiece(T, destinyT);
            }

            // #jogadaespecial roque grande
            if (p is King && destiny.column == origin.column - 2)
            {
                Position originT = new Position(origin.line, origin.column - 4);
                Position destinyT = new Position(origin.line, origin.column - 1);
                Piece T = board.removePiece(originT);
                T.incrementNumberOfMoves();
                board.putPiece(T, destinyT);
            }

            // #jogadaespecial en passant
            if (p is Peon)
            {
                if (origin.column != destiny.column && capturedPiece == null)
                {
                    Position posP;
                    if (p.color == Color.White)
                    {
                        posP = new Position(destiny.line + 1, destiny.column);
                    }
                    else
                    {
                        posP = new Position(destiny.line - 1, destiny.column);
                    }
                    capturedPiece = board.removePiece(posP);
                    captured.Add(capturedPiece);
                }
            }

            return capturedPiece;
        }

        public void undoMoviment(Position origin, Position destiny, Piece capturedPiece)
        {
            Piece p = board.removePiece(destiny);
            p.decrementNumberOfMoves();
            if (capturedPiece != null)
            {
                board.putPiece(capturedPiece, destiny);
                captured.Remove(capturedPiece);
            }
            board.putPiece(p, origin);

            // #jogadaespecial roque pequeno
            if (p is King && destiny.column == origin.column + 2)
            {
                Position originT = new Position(origin.line, origin.column + 3);
                Position destinyT = new Position(origin.line, origin.column + 1);
                Piece T = board.removePiece(destinyT);
                T.decrementNumberOfMoves();
                board.putPiece(T, originT);
            }

            // #jogadaespecial roque grande
            if (p is King && destiny.column == origin.column - 2)
            {
                Position originT = new Position(origin.line, origin.column - 4);
                Position destinyT = new Position(origin.line, origin.column - 1);
                Piece T = board.removePiece(destinyT);
                T.decrementNumberOfMoves();
                board.putPiece(T, originT);
            }

            // #jogadaespecial en passant
            if (p is Peon)
            {
                if (origin.column != destiny.column && capturedPiece == vulnerableEnPassant)
                {
                    Piece peon = board.removePiece(destiny);
                    Position posP;
                    if (p.color == Color.White)
                    {
                        posP = new Position(3, destiny.column);
                    }
                    else
                    {
                        posP = new Position(4, destiny.column);
                    }
                    board.putPiece(peon, posP);
                }
            }
        }

        public void makeMove(Position origin, Position destiny)
        {
            Piece capturedPiece = performMoviment(origin, destiny);

            if (isInCheck(currentPlayer))
            {
                undoMoviment(origin, destiny, capturedPiece);
                throw new BoardException("Você não pode se colocar em xeque!");
            }

            if (isInCheck(opponent(currentPlayer)))
            {
                check = true;
            }
            else
            {
                check = false;
            }

            if (testCheckMate(opponent(currentPlayer)))
            {
                finished = true;
            }
            else
            {
                turn++;
                changePlayer();
            }

            Piece p = board.piece(destiny);
            // #jogadaespecial en passant
            if (p is Peon && (destiny.line == origin.line - 2 || destiny.line == origin.line + 2))
            {
                vulnerableEnPassant = p;
            }
            else
            {
                vulnerableEnPassant = null;
            }
        }

        public void validateHomePosition(Position pos)
        {
            if (board.piece(pos) == null)
            {
                throw new BoardException("Não existe peça na posição de origem escolhida!");
            }
            if (currentPlayer != board.piece(pos).color)
            {
                throw new BoardException("A peça de origem escolhida não é sua!");
            }
            if (!board.piece(pos).existPossibleMovement())
            {
                throw new BoardException("Não há movimentos possíveis para a peça de origem escolhida!");
            }
        }

        public void validateTargetPosition(Position origin, Position destiny)
        {
            if (!board.piece(origin).possibleMove(destiny))
            {
                throw new BoardException("Posição de destino inválida!");
            }
        }


        private void changePlayer()
        {
            if (currentPlayer == Color.White)
            {
                currentPlayer = Color.Black;
            }
            else
            {
                currentPlayer = Color.White;
            }
        }

        public HashSet<Piece> capturedGamePieces(Color color)
        {
            HashSet<Piece> aux = new HashSet<Piece>();
            foreach (Piece x in captured)
            {
                if (x.color == color)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Piece> piecesInGame(Color color)
        {
            HashSet<Piece> aux = new HashSet<Piece>();
            foreach (Piece x in pieces)
            {
                if (x.color == color)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(capturedGamePieces(color));
            return aux;
        }

        private Color opponent(Color color)
        {
            if (color == Color.White)
            {
                return Color.Black;
            }
            else
            {
                return Color.White;
            }
        }

        private Piece king(Color color)
        {
            foreach (Piece x in piecesInGame(color))
            {
                if (x is King)
                {
                    return x;
                }
            }
            return null;
        }

        public bool isInCheck(Color color)
        {
            Piece K = king(color);
            if (K == null)
            {
                throw new BoardException("Não tem rei da cor " + color + " no tabuleiro!");
            }

            foreach (Piece x in piecesInGame(opponent(color)))
            {
                bool[,] mat = x.possibleMoves();
                if (mat[K.position.line, K.position.column])
                {
                    return true;
                }
            }
            return false;
        }

        public bool testCheckMate(Color color)
        {
            if (!isInCheck(color))
            {
                return false;
            }
            foreach (Piece x in piecesInGame(color))
            {
                bool[,] mat = x.possibleMoves();
                for (int i = 0; i < board.lines; i++)
                {
                    for (int j = 0; j < board.columns; j++)
                    {
                        if (mat[i, j])
                        {
                            Position origin = x.position;
                            Position destiny = new Position(i, j);
                            Piece capturedPiece = performMoviment(origin, destiny);
                            bool testCheck = isInCheck(color);
                            undoMoviment(origin, destiny, capturedPiece);
                            if (!testCheck)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;

        }

        public void placeNewPiece(char column, int line, Piece piece)
        {
            board.putPiece(piece, new PositionChess(column, line).toPosition());
            pieces.Add(piece);
        }

        private void placePieces()
        {
            placeNewPiece('a', 1, new Tower(board, Color.White));
            placeNewPiece('b', 1, new Horse(board, Color.White));
            placeNewPiece('c', 1, new Bishop(board, Color.White));
            placeNewPiece('d', 1, new Queen(board, Color.White));
            placeNewPiece('e', 1, new King(board, Color.White, this));
            placeNewPiece('f', 1, new Bishop(board, Color.White));
            placeNewPiece('g', 1, new Horse(board, Color.White));
            placeNewPiece('h', 1, new Tower(board, Color.White));
            placeNewPiece('a', 2, new Peon(board, Color.White, this));
            placeNewPiece('b', 2, new Peon(board, Color.White, this));
            placeNewPiece('c', 2, new Peon(board, Color.White, this));
            placeNewPiece('d', 2, new Peon(board, Color.White, this));
            placeNewPiece('e', 2, new Peon(board, Color.White, this));
            placeNewPiece('f', 2, new Peon(board, Color.White, this));
            placeNewPiece('g', 2, new Peon(board, Color.White, this));
            placeNewPiece('h', 2, new Peon(board, Color.White, this));

            placeNewPiece('a', 8, new Tower(board, Color.Black));
            placeNewPiece('b', 8, new Horse(board, Color.Black));
            placeNewPiece('c', 8, new Bishop(board, Color.Black));
            placeNewPiece('d', 8, new Queen(board, Color.Black));
            placeNewPiece('e', 8, new King(board, Color.Black, this));
            placeNewPiece('f', 8, new Bishop(board, Color.Black));
            placeNewPiece('g', 8, new Horse(board, Color.Black));
            placeNewPiece('h', 8, new Tower(board, Color.Black));
            placeNewPiece('a', 7, new Peon(board, Color.Black, this));
            placeNewPiece('b', 7, new Peon(board, Color.Black, this));
            placeNewPiece('c', 7, new Peon(board, Color.Black, this));
            placeNewPiece('d', 7, new Peon(board, Color.Black, this));
            placeNewPiece('e', 7, new Peon(board, Color.Black, this));
            placeNewPiece('f', 7, new Peon(board, Color.Black, this));
            placeNewPiece('g', 7, new Peon(board, Color.Black, this));
            placeNewPiece('h', 7, new Peon(board, Color.Black, this));
        }
    }
}
