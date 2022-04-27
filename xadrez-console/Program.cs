using System;
using board;
using chess;

namespace xadrez_console
{
    class Program
    {
        static void Main(string[] args)
        {
            
            try
            {
                ChessGame game = new ChessGame();
                
                while (!game.finished)
                {
                    Console.Clear();
                    Screen.printBoard(game.board);

                    Console.WriteLine();
                    Console.Write("Origin: ");
                    Position origin = Screen.readPositionChess().toPosition();

                    bool[,] possiblePositions = game.board.piece(origin).possibleMoves();

                    Console.Clear();
                    Screen.printBoard(game.board, possiblePositions);

                    Console.WriteLine();
                    Console.Write("Destiny: ");
                    Position destiny = Screen.readPositionChess().toPosition();

                    game.performMoviment(origin, destiny);
                }                
            }
            catch (BoardException e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
