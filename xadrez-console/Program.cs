﻿using System;
using board;

namespace xadrez_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Position P;

            P = new Position(3, 4);

            Console.WriteLine("Position: " + P);
        }
    }
}
