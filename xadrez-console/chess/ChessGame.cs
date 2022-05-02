﻿using System.Collections.Generic;
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
        public bool xeque { get; private set; }

        public ChessGame()
        {
            board = new Board(8, 8);
            turn = 1;
            currentPlayer = Color.White;
            finished = false;
            xeque = false;
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
                xeque = true;
            }
            else
            {
                xeque = false;
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
            placeNewPiece('a', 8, new King(board, Color.Black));
            placeNewPiece('b', 8, new Tower(board, Color.Black));
            placeNewPiece('h', 7, new Tower(board, Color.White));
            placeNewPiece('c', 1, new Tower(board, Color.White));
            placeNewPiece('d', 1, new King(board, Color.White));

            /*
            placeNewPiece('c', 1, new Tower(board, Color.White));
            placeNewPiece('c', 2, new Tower(board, Color.White));
            placeNewPiece('d', 2, new Tower(board, Color.White));
            placeNewPiece('e', 1, new Tower(board, Color.White));
            placeNewPiece('e', 2, new Tower(board, Color.White));
            placeNewPiece('d', 1, new King(board, Color.White));

            placeNewPiece('c', 8, new Tower(board, Color.Black));
            placeNewPiece('c', 7, new Tower(board, Color.Black));
            placeNewPiece('d', 7, new Tower(board, Color.Black));
            placeNewPiece('e', 8, new Tower(board, Color.Black));
            placeNewPiece('e', 7, new Tower(board, Color.Black));
            placeNewPiece('d', 8, new King(board, Color.Black));
            */
            /*
            board.putPiece(new Tower(board, Color.Black), new PositionChess('a', 8).toPosition());
            board.putPiece(new Horse(board, Color.Black), new PositionChess('b', 8).toPosition());
            board.putPiece(new Bishop(board, Color.Black), new PositionChess('c', 8).toPosition());
            board.putPiece(new Queen(board, Color.Black), new PositionChess('d', 8).toPosition());
            board.putPiece(new King(board, Color.Black), new PositionChess('e', 8).toPosition());
            board.putPiece(new Bishop(board, Color.Black), new PositionChess('f', 8).toPosition());
            board.putPiece(new Horse(board, Color.Black), new PositionChess('g', 8).toPosition());
            board.putPiece(new Tower(board, Color.Black), new PositionChess('h', 8).toPosition());
            board.putPiece(new Peon(board, Color.Black), new PositionChess('a', 7).toPosition());
            board.putPiece(new Peon(board, Color.Black), new PositionChess('b', 7).toPosition());
            board.putPiece(new Peon(board, Color.Black), new PositionChess('c', 7).toPosition());
            board.putPiece(new Peon(board, Color.Black), new PositionChess('d', 7).toPosition());
            board.putPiece(new Peon(board, Color.Black), new PositionChess('e', 7).toPosition());
            board.putPiece(new Peon(board, Color.Black), new PositionChess('f', 7).toPosition());
            board.putPiece(new Peon(board, Color.Black), new PositionChess('g', 7).toPosition());
            board.putPiece(new Peon(board, Color.Black), new PositionChess('h', 7).toPosition());

            board.putPiece(new Tower(board, Color.White), new PositionChess('a', 1).toPosition());
            board.putPiece(new Horse(board, Color.White), new PositionChess('b', 1).toPosition());
            board.putPiece(new Bishop(board, Color.White), new PositionChess('c', 1).toPosition());
            board.putPiece(new King(board, Color.White), new PositionChess('d', 1).toPosition());
            board.putPiece(new Queen(board, Color.White), new PositionChess('e', 1).toPosition());
            board.putPiece(new Bishop(board, Color.White), new PositionChess('f', 1).toPosition());
            board.putPiece(new Horse(board, Color.White), new PositionChess('g', 1).toPosition());
            board.putPiece(new Tower(board, Color.White), new PositionChess('h', 1).toPosition());
            board.putPiece(new Peon(board, Color.White), new PositionChess('a', 2).toPosition());
            board.putPiece(new Peon(board, Color.White), new PositionChess('b', 2).toPosition());
            board.putPiece(new Peon(board, Color.White), new PositionChess('c', 2).toPosition());
            board.putPiece(new Peon(board, Color.White), new PositionChess('d', 2).toPosition());
            board.putPiece(new Peon(board, Color.White), new PositionChess('e', 2).toPosition());
            board.putPiece(new Peon(board, Color.White), new PositionChess('f', 2).toPosition());
            board.putPiece(new Peon(board, Color.White), new PositionChess('g', 2).toPosition());
            board.putPiece(new Peon(board, Color.White), new PositionChess('h', 2).toPosition());
            */
        }
    }
}
