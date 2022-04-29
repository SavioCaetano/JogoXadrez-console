using System;
using System.Collections.Generic;
using board;
using chess;

namespace xadrez_console
{
    class Screen
    {
        public static void printGame(ChessGame game)
        {
            printBoard(game.board);
            Console.WriteLine();
            printCapturedPieces(game);            
            Console.WriteLine("Turno: " + game.turn);
            Console.WriteLine("Aguardando jogada: " + game.currentPlayer);
            if (game.xeque)
            {
                Console.WriteLine("XEQUE!");
            }
            Console.WriteLine();
        }

        public static void printCapturedPieces(ChessGame game)
        {
            Console.WriteLine("Peças capturadas:");
            Console.Write("Brancas: ");
            printSetOfPiece(game.capturedGamePieces(Color.White));

            ConsoleColor aux = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Pretas: ");            
            printSetOfPiece(game.capturedGamePieces(Color.Black));
            Console.ForegroundColor = aux;
            
            Console.WriteLine();
        }

        public static void printSetOfPiece(HashSet<Piece> setOfPiece)
        {
            Console.Write("[");
            foreach(Piece i in setOfPiece)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("]");
        }
        
        public static void printBoard(Board board)
        {
            for (int i = 0; i < board.lines; i++)
            {
                Console.Write(8 - i + " ");
                for (int j = 0; j < board.columns; j++)
                {
                    printPiece(board.piece(i, j));
                }
                Console.WriteLine();
            }
            Console.WriteLine("  a b c d e f g h");
        }

        public static void printBoard(Board board, bool[,] possiblePositions)
        {
            ConsoleColor originalBackground = Console.BackgroundColor;
            ConsoleColor changedBackground = ConsoleColor.DarkGray;

            for (int i = 0; i < board.lines; i++)
            {
                Console.Write(8 - i + " ");
                for (int j = 0; j < board.columns; j++)
                {
                    if (possiblePositions[i,j])
                    {
                        Console.BackgroundColor = changedBackground;
                    }
                    else
                    {
                        Console.BackgroundColor = originalBackground;
                    }
                    printPiece(board.piece(i, j));
                    Console.BackgroundColor = originalBackground;
                }
                Console.WriteLine();
            }
            Console.WriteLine("  a b c d e f g h");
            Console.BackgroundColor = originalBackground;
        }

        public static PositionChess readPositionChess()
        {
            string s = Console.ReadLine();
            char column = s[0];
            int line = int.Parse(s[1] + "");
            return new PositionChess(column, line);
        }

        public static void printPiece(Piece piece)
        {
            if (piece == null)
            {
                Console.Write("- ");
            }
            else
            {
                if (piece.color == Color.White)
                {
                    Console.Write(piece);
                }
                else if (piece.color == Color.Black)
                {
                    ConsoleColor aux = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(piece);
                    Console.ForegroundColor = aux;
                }
                Console.Write(" ");
            }
        }
    }
}
