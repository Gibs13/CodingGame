using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace summer_bot;

/**
 * Connect towns with your train tracks and disrupt the opponent's.
 **/
class Player
{
    static Dictionary<int, Town> townsDictionary;
    static Dictionary<int, Region> regionsDictionary;

    static int myScore = 0;
    static int foeScore = 0;
    static int myId = -1;

    static int foeId() => 1 - myId;

    static void Main(string[] args)
    {
        /// 21 ≤ width ≤ 30
        /// 14 ≤ height ≤ 20 
        /// 4 ≤ townCount ≤ 12
        /// nombre de param entree :
        /// 30 * 20 * 2 + 12 * 12 
        /// 
        /// nombre de param sortie :
        /// 30 * 20 * 2
        /// 
        /// 

        string[] inputs;
        myId = int.Parse(Console.ReadLine()); // 0 or 1

        townsDictionary = new();
        regionsDictionary = new();

        Map.width = int.Parse(Console.ReadLine()); // map size
        Map.height = int.Parse(Console.ReadLine());
        Map.tiles = new Tile[Map.width, Map.height];
        for (int i = 0; i < Map.width; i++)
        {
            for (int j = 0; j < Map.height; j++)
            {
                inputs = Console.ReadLine().Split(' ');
                int regionId = int.Parse(inputs[0]);
                Map.tiles[i, j] = new Tile
                {
                    regionId = regionId,
                    terrain = int.Parse(inputs[1]), // 0 (PLAINS), 1 (RIVER), 2 (MOUNTAIN), 3 (POI)
                };
                if (regionsDictionary.ContainsKey(regionId))
                {
                    regionsDictionary[regionId].tilesCoords.Add((i, j));
                }
                else
                {
                    regionsDictionary.Add(
                        regionId,
                        new Region
                        {
                            tilesCoords = [(i, j)],
                            inked = false,
                            instability = 0,
                            hasCity = false,
                        }
                    );
                }
            }
        }

        int townCount = int.Parse(Console.ReadLine());

        for (int i = 0; i < townCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int regionId = Map.tiles[int.Parse(inputs[1]), int.Parse(inputs[2])].regionId;
            townsDictionary.Add(
                int.Parse(inputs[0]),
                new Town
                {
                    regionId = regionId,
                    w = int.Parse(inputs[1]),
                    h = int.Parse(inputs[2]),
                    desiredConnections =
                        inputs[3] == "x"
                            ? new List<int>()
                            : inputs[3].Split(',').Select(x => Int32.Parse(x)).ToList(),
                }
            );
            regionsDictionary[regionId].hasCity = true;
        }

        // game loop
        while (true)
        {
            myScore = int.Parse(Console.ReadLine());
            foeScore = int.Parse(Console.ReadLine());

            int disruptH = -1;
            int disruptW = -1;

            foreach (Town vil in townsDictionary.Values)
            {
                vil.connected = false;
            }

            for (int i = 0; i < Map.width; i++)
            {
                for (int j = 0; j < Map.height; j++)
                {
                    inputs = Console.ReadLine().Split(' ');

                    int regionId = Map.tiles[i, j].regionId;
                    regionsDictionary[regionId].instability = int.Parse(inputs[1]); // region inked (destroyed) when this >= 3.
                    regionsDictionary[regionId].inked = inputs[2] != "0"; // true if region is destroyed.

                    Map.tiles[i, j].tracksOwner = int.Parse(inputs[0]);
                    string partOfActiveConnections = inputs[3]; // if this cell is part of one or more railway connections, this will be town ids (separated by -) in a list separated by commas. e.g. 0-1,1-2,1-3. "x" otherwise.

                    Map.tiles[i, j].numberOfActiveConnections = 0;
                    if (partOfActiveConnections != "x")
                    {
                        string[] connectionArray = partOfActiveConnections.Split(',');
                        foreach (string connection in connectionArray)
                        {
                            Map.tiles[i, j].numberOfActiveConnections++;
                            string[] cs = connection.Split('-');
                            townsDictionary[Int32.Parse(cs[0])].connected = true;
                            townsDictionary[Int32.Parse(cs[1])].connected = true;
                        }
                    }
                }
            }

            // order of preference : NORTH - EAST - SOUTH - WEST

            // AUTOPLACE x1 y1 x2 y2 | PLACE_TRACKS x y | DISRUPT regionId | MESSAGE text

            Town v = townsDictionary.Values.FirstOrDefault(x =>
                !x.connected && x.desiredConnections.Count() > 0
            );
            if (v != null)
            {
                Town d = townsDictionary[v.desiredConnections.First()];
                Console.Write("AUTOPLACE " + v.w + " " + v.h + " " + d.w + " " + d.h);
            }
            else
                Console.Write(
                    "AUTOPLACE " + 0 + " " + 0 + " " + (Map.width - 1) + " " + (Map.height - 1)
                );
            if (disruptH != -1)
                Console.Write(";DISRUPT " + disruptW + " " + disruptH);
            Console.WriteLine();
        }
    }

    static decimal Evaluate()
    {
        // win = 1 ;
        // lose = 0
        return 0;
    }
}

class Town
{
    public int townId;
    public int regionId;
    public int w;
    public int h;
    public List<int> desiredConnections;
    public bool connected;
    public List<int> desiredBy;
}

static class Map
{
    public static int width;
    public static int height;
    public static Tile[,] tiles;
}

public class Tile
{
    public int w;
    public int h;
    public int terrain;
    public int regionId;
    public int tracksOwner;
    public int numberOfActiveConnections;
}

public class Region
{
    public List<(int, int)> tilesCoords;
    public bool hasCity;
    public int instability;
    public bool inked;
}
