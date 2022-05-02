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
                    try
                    {
                        Console.Clear();
                        Screen.printGame(game);
                                             
                        Console.Write("Origem: ");
                        Position origin = Screen.readPositionChess().toPosition();
                        game.validateHomePosition(origin);

                        bool[,] possiblePositions = game.board.piece(origin).possibleMoves();

                        Console.Clear();
                        Screen.printBoard(game.board, possiblePositions);

                        Console.WriteLine();
                        Console.Write("Destino: ");
                        Position destiny = Screen.readPositionChess().toPosition();
                        game.validateTargetPosition(origin, destiny);

                        game.makeMove(origin, destiny);
                    }
                    catch (BoardException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }                    
                }
                Console.Clear();
                Screen.printGame(game);
            }
            catch (BoardException e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
