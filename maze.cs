using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static char UP = 'A';
    static char DOWN = 'E';
    static char RIGHT = 'D';
    static char LEFT = 'C';

    static List<(int, int)> entitiesCoord;

    static void Main(string[] args)
    {
        int maxX = int.Parse(Console.ReadLine());
        int maxY = int.Parse(Console.ReadLine());
        int numberOfEntities = int.Parse(Console.ReadLine());

        // Dictionary<int,List<int>> nodesToExplore = new(); // later
        bool[,] exploredNodes = new bool[maxX, maxY];
        (int, int) lastCoord = (-1, -1);

        Console.Error.WriteLine(maxX + " " + maxY + " " + numberOfEntities);

        // game loop
        while (true)
        {
            string left = Console.ReadLine();
            string up = Console.ReadLine();
            string right = Console.ReadLine();
            string down = Console.ReadLine();
            Console.Error.WriteLine(" " + up + " ");
            Console.Error.WriteLine(left + " " + right);
            Console.Error.WriteLine(" " + down + " ");

            (int, int) meCoord = (-1, -1);

            entitiesCoord = new();

            for (int i = 0; i < numberOfEntities; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]);
                if (i != numberOfEntities - 1)
                    entitiesCoord.Add((x, y));
                else
                    meCoord = (x, y); // I think i am the last one
                Console.Error.WriteLine("  " + x + " " + y);
            }

            // avoid moving entities
            // TODO avoid when passing through walls

            for (int i = 0; i < numberOfEntities - 1; i++)
            {
                if (left == "_")
                {
                    if (
                        meCoord.Left().IsEqual(entitiesCoord[i])
                        || meCoord.Left().IsEqual(entitiesCoord[i].Left())
                        || meCoord.Left().IsEqual(entitiesCoord[i].Right())
                        || meCoord.Left().IsEqual(entitiesCoord[i].Down())
                        || meCoord.Left().IsEqual(entitiesCoord[i].Up())
                    )
                    {
                        left = "#";
                        Console.Error.WriteLine("entity detected left");
                    }
                }
                if (right == "_")
                {
                    if (
                        meCoord.Right().IsEqual(entitiesCoord[i])
                        || meCoord.Right().IsEqual(entitiesCoord[i].Left())
                        || meCoord.Right().IsEqual(entitiesCoord[i].Right())
                        || meCoord.Right().IsEqual(entitiesCoord[i].Down())
                        || meCoord.Right().IsEqual(entitiesCoord[i].Up())
                    )
                    {
                        right = "#";
                        Console.Error.WriteLine("entity detected right");
                    }
                }
                if (down == "_")
                {
                    if (
                        meCoord.Down().IsEqual(entitiesCoord[i])
                        || meCoord.Down().IsEqual(entitiesCoord[i].Left())
                        || meCoord.Down().IsEqual(entitiesCoord[i].Right())
                        || meCoord.Down().IsEqual(entitiesCoord[i].Down())
                        || meCoord.Down().IsEqual(entitiesCoord[i].Up())
                    )
                    {
                        down = "#";
                        Console.Error.WriteLine("entity detected down");
                    }
                }
                if (up == "_")
                {
                    if (
                        meCoord.Up().IsEqual(entitiesCoord[i])
                        || meCoord.Up().IsEqual(entitiesCoord[i].Left())
                        || meCoord.Up().IsEqual(entitiesCoord[i].Right())
                        || meCoord.Up().IsEqual(entitiesCoord[i].Down())
                        || meCoord.Up().IsEqual(entitiesCoord[i].Up())
                    )
                    {
                        up = "#";
                        Console.Error.WriteLine("entity detected up");
                    }
                }
            }

            // choose move
            char c = 'B';

            if (!exploredNodes[meCoord.Item1, meCoord.Item2])
            {
                exploredNodes[meCoord.Item1, meCoord.Item2] = true;
            }

            // prioritize unexplored tiles
            if (
                left == "_"
                && (
                    meCoord.Item2 - 1 >= 0 && !exploredNodes[meCoord.Item1, meCoord.Item2 - 1]
                    || meCoord.Item2 - 1 < 0 && !exploredNodes[meCoord.Item1, maxY - 1]
                )
            )
            {
                c = LEFT;
            }
            else if (
                right == "_"
                && (
                    meCoord.Item2 + 1 < maxY && !exploredNodes[meCoord.Item1, meCoord.Item2 + 1]
                    || meCoord.Item2 + 1 == maxY && !exploredNodes[meCoord.Item1, 0]
                )
            )
            {
                c = RIGHT;
            }
            else if (
                down == "_"
                && (
                    meCoord.Item1 - 1 >= 0 && !exploredNodes[meCoord.Item1 - 1, meCoord.Item2]
                    || meCoord.Item1 - 1 < 0 && !exploredNodes[maxX, meCoord.Item2]
                )
            )
            {
                c = DOWN;
            }
            else if (
                up == "_"
                && (
                    meCoord.Item1 + 1 < maxX && !exploredNodes[meCoord.Item1 + 1, meCoord.Item2]
                    || meCoord.Item1 + 1 == maxX && !exploredNodes[0, meCoord.Item2]
                )
            )
            {
                c = UP;
            }
            // Avoid going back
            else if (left == "_" && !lastCoord.IsEqual(meCoord.Left()))
            {
                c = LEFT;
            }
            else if (right == "_" && !lastCoord.IsEqual(meCoord.Right()))
            {
                c = RIGHT;
            }
            else if (down == "_" && !lastCoord.IsEqual(meCoord.Down()))
            {
                c = DOWN;
            }
            else if (up == "_" && !lastCoord.IsEqual(meCoord.Up()))
            {
                c = UP;
            }
            // Move anyway
            else if (left == "_")
            {
                c = LEFT;
            }
            else if (right == "_")
            {
                c = RIGHT;
            }
            else if (down == "_")
            {
                c = DOWN;
            }
            else if (up == "_")
            {
                c = UP;
            }

            Console.WriteLine(c);
            lastCoord = meCoord;
        }
    }
}

public static class CoordExtension
{
    public static bool IsEqual(this (int, int) coord1, (int, int) coord2)
    {
        return coord1.Item1 == coord2.Item1 && coord1.Item2 == coord2.Item2;
    }

    public static (int, int) Right(this (int, int) coord1)
    {
        return (coord1.Item1, coord1.Item2 + 1);
    }

    public static (int, int) Left(this (int, int) coord1)
    {
        return (coord1.Item1, coord1.Item2 - 1);
    }

    public static (int, int) Up(this (int, int) coord1)
    {
        return (coord1.Item1 + 1, coord1.Item2);
    }

    public static (int, int) Down(this (int, int) coord1)
    {
        return (coord1.Item1 - 1, coord1.Item2);
    }
}
