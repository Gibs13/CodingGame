using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

namespace ghost_cell_bot2;

public static class WorldStatus
{
    public static int CurrentTurn { get; set; }
    public static int[,] DistanceArray; // distance between two factories
    public static (int, List<int>)[,] ClosestPathArray; // distance between two factories and the list of intermediate nodes
    public static int FactoryCount;
}

public class GameState
{
    public static GameState currentState;
    public Factory[] Factories;
    public List<int> alliedFactories;
    public List<int> ennemyFactories;
    public int AlliedTotalTroops;
    public int EnnemyTotalTroops;
    public int AlliedTotalProduction;
    public int EnnemyTotalProduction;
    public int alliedBombsLeft;
    public int ennemyBombsLeft;
    public List<Troop> troopsSended;
    public List<Bomb> bombsSended;
}

public static class Gamerules
{
    // Game rules
    public const int maxNumberOfBombs = 2;
    public const int bombNumberOfTurnsDisactivated = 5;
    public const int maxMSTimeForATurn = 50;
    public const int maxMSTimeForFirstTurn = 1000;
    public const int maxNumberOfTurns = 200;
    public const int minDistanceBetweenTwoFactories = 1;
    public const int maxDistanceBetweenTwoFactories = 20;
    public const int minNumberOfFactories = 7;
    public const int maxNumberOfFactories = 15;
    public const int minNumberOfTroopsInStartingBase = 15;
    public const int maxNumberOfTroopsInStartingBase = 30;
    public const int minFactoryProduction = 0;
    public const int maxFactoryProduction = 3;
    public const int troopCostForIncreasingProduction = 10;
    public const int ennemyBombTarget = -1;
    public const int me = 1;
    public const int neutral = 0;
    public const int them = -1;
}

public class Factory
{
    public int id;
    public int owner;
    public int troops;
    public int production;
    public int downtime;
    public List<int> incomingTroops;

    public Factory(int id, int owner, int troops, int production, int downtime)
    {
        this.id = id;
        this.owner = owner;
        this.troops = troops;
        this.production = production;
        this.downtime = downtime;
        this.incomingTroops = new List<int>();
    }

    public void Update(int owner, int troops, int production, int downtime)
    {
        this.owner = owner;
        this.troops = troops;
        this.production = production;
        this.downtime = downtime;
        this.incomingTroops = new List<int>();
    }

    public void FindPathsToFactory(int targetId)
    {
        List<Factory> nodesToExplore = new List<Factory>();
        // foreach (int )
        { }
    }

    public int DistanceToFactory(int targetId)
    {
        return WorldStatus.DistanceArray[this.id, targetId];
    }

    public void AddIncomingTroop(int troopId)
    {
        this.incomingTroops.Add(troopId);
    }
}

public class Troop
{
    public int id;
    public int owner;
    public int origin;
    public int target;
    public int number;
    public int delay;

    public Troop(int id, int owner, int origin, int target, int number, int delay)
    {
        this.id = id;
        this.owner = owner;
        this.origin = origin;
        this.target = target;
        this.number = number;
        this.delay = delay;
    }
}

public class Bomb
{
    public int id;
    public int owner;
    public int origin;
    public int target;
    public int delay;
    public int launchedTurn;

    public Bomb(int id, int owner, int origin, int target, int delay)
    {
        this.id = id;
        this.owner = owner;
        this.origin = origin;
        this.target = target;
        this.delay = delay;
        this.launchedTurn = WorldStatus.CurrentTurn;
        if (this.delay == 1)
            this.id = -1;
    }

    public void Update()
    {
        this.delay -= 1;
        if (this.delay == 1)
            this.id = -1;
        if (this.owner == Gamerules.them)
        {
            Console.Error.WriteLine(" bomb " + this.target + " dans " + this.delay);
        }
    }
}

// possible orders
// WAIT
// MOVE origin target number
// BOMB origin target -- cannot move and bomb form same origin to same target at the same turn
// INC target
public abstract class Order
{
    public abstract string toString();
}

public class MoveOrder : Order
{
    public int alliedFactoryId;
    public int targetFactoryId;
    public int troopNumber;

    public override string toString()
    {
        return "MOVE " + this.alliedFactoryId + " " + this.targetFactoryId + " " + this.troopNumber;
    }
}

public class BombOrder : Order
{
    public int alliedFactoryId;
    public int targetFactoryId;

    public override string toString()
    {
        return "BOMB " + this.alliedFactoryId + " " + this.targetFactoryId;
    }
}

public class IncreaseOrder : Order
{
    public int alliedFactoryId;

    public override string toString()
    {
        return "INC " + this.alliedFactoryId;
    }
}

public class Objective
{
    public int targetFactoryId;
    public int requiredTroops;
    public int distance;
    public int reward;
    public int cost;

    public Objective(int targetFactoryId, int requiredTroops, int distance, int reward, int cost)
    {
        this.targetFactoryId = targetFactoryId;
        this.requiredTroops = requiredTroops;
        this.distance = distance;
        this.reward = reward;
        this.cost = cost;
    }
}

public static class Bot
{
    public static bool logObjectives = false;

    public static void ClosestPathToEnnemy()
    {
        bool ordersent = false;
        for (int i = 0; i < WorldStatus.ClosestPathArray[1, 2].Item2.Count; i++)
        {
            if (
                GameState.currentState.Factories[WorldStatus.ClosestPathArray[1, 2].Item2[i]].owner
                == Gamerules.me
            )
            {
                Console.Write(
                    (ordersent ? ";" : "")
                        + "MOVE "
                        + WorldStatus.ClosestPathArray[1, 2].Item2[i]
                        + " "
                        + (
                            i == WorldStatus.ClosestPathArray[1, 2].Item2.Count - 1
                                ? "2"
                                : WorldStatus.ClosestPathArray[1, 2].Item2[i + 1]
                        )
                        + " "
                        + GameState
                            .currentState
                            .Factories[WorldStatus.ClosestPathArray[1, 2].Item2[i]]
                            .troops
                );
                ordersent = true;
            }
        }

        if (ordersent)
            Console.WriteLine();
        else
            Console.WriteLine("WAIT");
    }

    public static Dictionary<int, List<Objective>> GetTargetList(int myFactoryId)
    {
        Dictionary<int, List<Objective>> targetList = new Dictionary<int, List<Objective>>();
        for (int factoryId = 0; factoryId < WorldStatus.FactoryCount; factoryId++)
        {
            bool self = factoryId == myFactoryId;

            // REWARD CALCULATION

            int reward = GameState.currentState.Factories[factoryId].production;

            if (
                !self
                && GameState.currentState.Factories[factoryId].owner == Gamerules.them
                && GameState.currentState.Factories[factoryId].troops == 0
            )
                reward *= 3;

            if (self)
                reward *= 6;

            // PATH CALCULATION

            int firstnode = self
                ? myFactoryId
                : WorldStatus.ClosestPathArray[myFactoryId, factoryId].Item2[1];

            // Avoid bombed path

            foreach (
                Bomb bomb in GameState.currentState.bombsSended.Where(bomb =>
                    bomb.target == firstnode
                )
            )
            {
                if (
                    bomb.owner == Gamerules.me
                    && (bomb.delay >= WorldStatus.DistanceArray[myFactoryId, firstnode])
                )
                    reward = -1;
                else if (
                    bomb.owner == Gamerules.them
                    && bomb.delay == WorldStatus.DistanceArray[myFactoryId, firstnode]
                )
                    reward = -1;
            }

            int cost = 0;
            foreach (
                int intermediateId in WorldStatus.ClosestPathArray[myFactoryId, factoryId].Item2
            )
            {
                if (GameState.currentState.Factories[intermediateId].owner == Gamerules.neutral)
                    cost += GameState.currentState.Factories[intermediateId].troops;
                if (
                    intermediateId != myFactoryId
                    && GameState.currentState.Factories[intermediateId].owner == Gamerules.me
                )
                    cost -= GameState.currentState.Factories[intermediateId].troops;
            }

            // REQUIRED TROOPS CALCULATION

            int requiredTroops = 0;
            if (!self && GameState.currentState.Factories[factoryId].downtime == 0)
                requiredTroops =
                    -GameState.currentState.Factories[factoryId].owner
                    * GameState.currentState.Factories[factoryId].production;

            if (GameState.currentState.Factories[factoryId].owner == Gamerules.neutral)
                requiredTroops = GameState.currentState.Factories[factoryId].troops + 1;

            if (
                GameState.currentState.Factories[factoryId].owner == Gamerules.them
                && GameState.currentState.Factories[factoryId].production > 0
            )
                requiredTroops += 1;

            List<Troop> incomingTroopList = GameState
                .currentState.troopsSended.Where(troop =>
                    GameState.currentState.Factories[factoryId].incomingTroops.Contains(troop.id)
                )
                .ToList();

            int[] incomingTroopArray = new int[Gamerules.maxDistanceBetweenTwoFactories];

            foreach (Troop incomingTroop in incomingTroopList)
            {
                if (incomingTroop != null && incomingTroop.owner == Gamerules.them)
                {
                    incomingTroopArray[incomingTroop.delay] += incomingTroop.number;
                }
                else if (incomingTroop != null && incomingTroop.owner == Gamerules.me)
                {
                    incomingTroopArray[incomingTroop.delay] -= incomingTroop.number;
                }
            }

            // ADDING INCOMING TROOPS and production

            int TempOwner = GameState.currentState.Factories[factoryId].owner;

            for (
                int i = 0;
                i
                    < (
                        self
                            ? Gamerules.maxDistanceBetweenTwoFactories
                            : WorldStatus.ClosestPathArray[myFactoryId, factoryId].Item1 + 1
                    );
                i++
            )
            {
                if (TempOwner == Gamerules.neutral)
                {
                    if ((requiredTroops -= Math.Abs(incomingTroopArray[i])) > 0)
                    {
                        requiredTroops = requiredTroops -= Math.Abs(incomingTroopArray[i]);
                    }
                    else
                    {
                        if (incomingTroopArray[i] < 0)
                        {
                            requiredTroops -= Math.Abs(incomingTroopArray[i]);
                            TempOwner = Gamerules.me;
                        }
                        else
                        {
                            requiredTroops = -requiredTroops + Math.Abs(incomingTroopArray[i]);
                            TempOwner = Gamerules.them;
                        }
                    }
                }
                else
                {
                    requiredTroops += incomingTroopArray[i];

                    // if (requiredTroops < 0)
                    //     TempOwner = Gamerules.me;
                    // else if (requiredTroops > 0)
                    //     TempOwner = Gamerules.them;

                    if (
                        !self
                        && requiredTroops <= 0
                        && GameState.currentState.Factories[factoryId].downtime <= i + 1
                    )
                        requiredTroops -=
                            TempOwner * GameState.currentState.Factories[factoryId].production;
                }
            }

            // remove incoming troops on the path
            foreach (
                int factoriesOnPath in WorldStatus.ClosestPathArray[myFactoryId, factoryId].Item2
            )
            {
                if (factoriesOnPath == myFactoryId)
                    continue;
                if (GameState.currentState.Factories[factoriesOnPath].owner == Gamerules.me)
                {
                    requiredTroops -=
                        GameState.currentState.Factories[factoriesOnPath].troops
                        + GameState
                            .currentState.Factories[factoriesOnPath]
                            .incomingTroops.Sum(incomingTroopid =>
                                GameState
                                    .currentState.troopsSended.Find(troop =>
                                        troop.id == incomingTroopid
                                    )
                                    .number
                            );
                }
            }

            // ADD ENNEMY TROOPS FROM NEARBY FACTORIES
            // int closestFactory = -1;
            // int closestFactoryDistance = Gamerules.maxDistanceBetweenTwoFactories;
            // foreach(int alliedFactory in GameState.currentState.alliedFactories)
            // {
            //     if(alliedFactory != factoryId && GameState.currentState.Factories[alliedFactory].DistanceToFactory(factoryId) < closestFactoryDistance)
            //     {
            //         closestFactory = alliedFactory;
            //         closestFactoryDistance = GameState.currentState.Factories[alliedFactory].DistanceToFactory(factoryId);
            //     }
            // }
            foreach (int ennemyFactory in GameState.currentState.ennemyFactories)
            {
                if (
                    GameState.currentState.Factories[ennemyFactory].DistanceToFactory(factoryId)
                    <= 2
                )
                {
                    requiredTroops += GameState.currentState.Factories[ennemyFactory].troops;
                }
            }

            if (requiredTroops < 0)
                requiredTroops = 0;

            if (requiredTroops > 0)
            {
                Objective objective = new Objective(
                    factoryId,
                    requiredTroops,
                    WorldStatus.ClosestPathArray[myFactoryId, factoryId].Item1,
                    reward,
                    cost
                );

                if (logObjectives)
                    Console.Error.WriteLine(
                        " OBJECTIF : DE "
                            + myFactoryId
                            + " POUR "
                            + factoryId
                            + " PASSANT PAR "
                            + firstnode
                            + " required troops = "
                            + requiredTroops
                            + " cout = "
                            + cost
                            + " reward = "
                            + reward
                    );

                if (targetList.ContainsKey(firstnode))
                    targetList[firstnode].Add(objective);
                else
                    targetList.Add(firstnode, new List<Objective> { objective });
            }
        }
        return targetList;
    }

    public static Dictionary<int, List<Objective>> GetBombTargetList()
    {
        Dictionary<int, List<Objective>> bombTargetList = new Dictionary<int, List<Objective>>();

        if (!GameState.currentState.alliedFactories.Any())
            return bombTargetList;

        int maxProduction = GameState.currentState.Factories.Max(f => f.production);

        // TODO : Si il y a un chemin plus proche que l'on emprunte, on attends
        // TODO : prendre en compte les troupes arrivantes pour savoir si on peut envoyer sur une troupe
        // On choisi des usines avec un reward d'au moins 15
        foreach (int factoryId in GameState.currentState.ennemyFactories)
        {
            if (GameState.currentState.Factories[factoryId].production >= maxProduction)
            {
                // On choisi notre usine la plus proche pour envoyer la bombe
                int closestFactoryId = GameState.currentState.alliedFactories.First(
                    (myFactoryId) =>
                        WorldStatus.DistanceArray[myFactoryId, factoryId]
                        == GameState.currentState.alliedFactories.Min(
                            (myFactoryId) => WorldStatus.DistanceArray[myFactoryId, factoryId]
                        )
                );

                Console.Error.WriteLine(" closest factory " + closestFactoryId);

                // Verifier qu'une bombe n'est pas déjà en route vers cette usine
                if (
                    (
                        GameState.currentState.Factories[factoryId].downtime > 0
                        && GameState.currentState.Factories[factoryId].downtime
                            > WorldStatus.DistanceArray[closestFactoryId, factoryId]
                    )
                    || GameState.currentState.bombsSended.Any(bomb =>
                        bomb.target == closestFactoryId
                        && bomb.owner == Gamerules.me
                        && (
                            bomb.delay > WorldStatus.DistanceArray[closestFactoryId, factoryId] - 5
                            || bomb.delay
                                < WorldStatus.DistanceArray[closestFactoryId, factoryId] + 5
                        )
                    )
                )
                {
                    continue;
                }

                // Verifier que des troupes ne sont pas déjà en route vers cette usine
                if (
                    WorldStatus.CurrentTurn > 1
                    && GameState
                        .currentState.Factories[factoryId]
                        .incomingTroops.Any(id =>
                        {
                            Troop troop = GameState.currentState.troopsSended.Find(troop =>
                                troop.id == id
                            );
                            return troop.owner == Gamerules.me
                                && troop.delay
                                    < WorldStatus.DistanceArray[closestFactoryId, factoryId];
                        })
                )
                {
                    continue;
                }

                Objective objective = new Objective(
                    factoryId,
                    -1,
                    WorldStatus.DistanceArray[closestFactoryId, factoryId],
                    15,
                    0
                );
                if (bombTargetList.ContainsKey(closestFactoryId))
                {
                    bombTargetList[closestFactoryId].Add(objective);
                }
                else
                {
                    bombTargetList.Add(closestFactoryId, new List<Objective> { objective });
                }
            }
        }
        return bombTargetList;
    }

    public static void ChooseOrders()
    {
        List<Order> orders = new List<Order>();

        if (GameState.currentState.alliedBombsLeft > 0)
        {
            Dictionary<int, List<Objective>> bombTargetList = GetBombTargetList();
            if (bombTargetList.Any())
            {
                orders.Add(
                    new BombOrder
                    {
                        alliedFactoryId = bombTargetList.First().Key,
                        targetFactoryId = bombTargetList.First().Value.First().targetFactoryId,
                    }
                );
                GameState.currentState.bombsSended.Add(
                    new Bomb(
                        -1,
                        Gamerules.me,
                        bombTargetList.First().Key,
                        bombTargetList.First().Value.First().targetFactoryId,
                        WorldStatus.DistanceArray[
                            bombTargetList.First().Key,
                            bombTargetList.First().Value.First().targetFactoryId
                        ]
                    )
                );
                GameState.currentState.alliedBombsLeft -= 1;
            }
        }

        List<MoveOrder> moveOrders = new List<MoveOrder>();

        foreach (int myFactoryId in GameState.currentState.alliedFactories)
        {
            Dictionary<int, List<Objective>> targetList = GetTargetList(myFactoryId);

            Console.Error.WriteLine(
                String.Join(
                    " ",
                    targetList
                        .Keys.OrderByDescending(key =>
                            targetList[key].Max(objective => objective.reward)
                        )
                        .ThenBy(key =>
                            targetList[key]
                                .Where(objective =>
                                    objective.reward
                                    == targetList[key].Max(objective => objective.reward)
                                )
                                .Min(objective => objective.cost)
                        )
                        .ThenBy(key => WorldStatus.DistanceArray[myFactoryId, key])
                )
            );

            foreach (
                int firstnode in targetList
                    .Keys.OrderByDescending(key =>
                        targetList[key].Max(objective => objective.reward)
                    )
                    .ThenBy(key =>
                        targetList[key]
                            .Where(objective =>
                                objective.reward
                                == targetList[key].Max(objective => objective.reward)
                            )
                            .Min(objective => objective.cost)
                    )
                    .ThenBy(key => WorldStatus.DistanceArray[myFactoryId, key])
            )
            {
                if (targetList[firstnode].Any(objective => objective.reward == -1))
                {
                    continue;
                }

                int totalRequiredTroops = (int)
                    targetList[firstnode]
                        .Sum(objective =>
                            objective.cost >= GameState.currentState.Factories[myFactoryId].troops
                                ? 0
                                : objective.requiredTroops
                        );

                if (totalRequiredTroops > 0)
                {
                    int sendableTroops = Math.Min(
                        GameState.currentState.Factories[myFactoryId].troops,
                        totalRequiredTroops
                    );
                    GameState.currentState.Factories[myFactoryId].troops -= sendableTroops;
                    // Console.Error.WriteLine(
                    //     " MOVE "
                    //         + myFactoryId
                    //         + " "
                    //         + firstnode
                    //         + " = "
                    //         + sendableTroops
                    //         + " reste "
                    //         + GameState.currentState.Factories[myFactoryId].troops
                    // );
                    if (myFactoryId != firstnode)
                        moveOrders.Add(
                            new MoveOrder
                            {
                                alliedFactoryId = myFactoryId,
                                targetFactoryId = firstnode,
                                troopNumber = sendableTroops,
                            }
                        );
                }
            }
        }

        // Do something of unused troops

        foreach (int myFactoryId in GameState.currentState.alliedFactories)
        {
            bool bombMaybeComing = GameState.currentState.bombsSended.Any(bomb =>
                bomb.owner == Gamerules.them && bomb.target == myFactoryId && bomb.delay == 1
            );
            if (GameState.currentState.Factories[myFactoryId].troops == 0)
                continue;
            else
            {
                int numberOfMoves = moveOrders.Count(order => order.alliedFactoryId == myFactoryId);
                int leftoverTroops = GameState.currentState.Factories[myFactoryId].troops;
                foreach (
                    MoveOrder moveOrder in moveOrders.Where(order =>
                        order.alliedFactoryId == myFactoryId
                    )
                )
                {
                    int troopsToMove = (int)Math.Ceiling((decimal)(leftoverTroops / numberOfMoves));
                    moveOrder.troopNumber += troopsToMove;
                    GameState.currentState.Factories[myFactoryId].troops -= troopsToMove;
                }
                if (bombMaybeComing && GameState.currentState.Factories[myFactoryId].troops > 0)
                {
                    if (moveOrders.Any(order => order.alliedFactoryId == myFactoryId))
                    {
                        moveOrders
                            .First(order => order.alliedFactoryId == myFactoryId)
                            .troopNumber += GameState.currentState.Factories[myFactoryId].troops;
                    }
                    else if (GameState.currentState.ennemyFactories.Any())
                    {
                        orders.Add(
                            new MoveOrder
                            {
                                alliedFactoryId = myFactoryId,
                                targetFactoryId = GameState.currentState.ennemyFactories.First(
                                    factoryId =>
                                        WorldStatus.DistanceArray[myFactoryId, factoryId]
                                        == GameState.currentState.ennemyFactories.Min(factoryId =>
                                            WorldStatus.DistanceArray[myFactoryId, factoryId]
                                        )
                                ),
                                troopNumber = GameState.currentState.Factories[myFactoryId].troops,
                            }
                        );
                    }
                }
                else
                    for (
                        int i = GameState.currentState.Factories[myFactoryId].production;
                        i < Gamerules.maxFactoryProduction;
                        i++
                    )
                        if (GameState.currentState.Factories[myFactoryId].troops > 20)
                        {
                            orders.Add(new IncreaseOrder { alliedFactoryId = myFactoryId });
                            GameState.currentState.Factories[myFactoryId].troops -= 10;
                        }
            }
        }

        orders.AddRange(moveOrders);

        bool orderSent = false;
        foreach (Order order in orders)
        {
            Console.Write((orderSent ? ";" : "") + order.toString());
            orderSent = true;
        }
        if (orderSent)
            Console.WriteLine();
        else
            Console.WriteLine("WAIT");
    }
}

class Player
{
    public static Stopwatch stopwatch = new Stopwatch();

    static void debugTime(string message = "")
    {
        Console.Error.WriteLine("TIME ELAPSED " + message + " : " + stopwatch.ElapsedMilliseconds);
    }

    static void resetTime()
    {
        stopwatch.Restart();
    }

    static void Initialisation()
    {
        WorldStatus.FactoryCount = int.Parse(Console.ReadLine()); // the number of factories
        stopwatch.Start();
        debugTime("Start");
        WorldStatus.CurrentTurn = 1;

        GameState.currentState = new GameState();
        GameState.currentState.alliedBombsLeft = GameState.currentState.ennemyBombsLeft =
            Gamerules.maxNumberOfBombs;

        GameState.currentState.bombsSended = new List<Bomb>();

        int linkCount = int.Parse(Console.ReadLine()); // the number of links between factories

        WorldStatus.DistanceArray = new int[WorldStatus.FactoryCount, WorldStatus.FactoryCount];
        GameState.currentState.Factories = new Factory[WorldStatus.FactoryCount];

        for (int i = 0; i < linkCount; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int factory1 = int.Parse(inputs[0]);
            int factory2 = int.Parse(inputs[1]);
            int distance = int.Parse(inputs[2]);
            WorldStatus.DistanceArray[factory1, factory2] = WorldStatus.DistanceArray[
                factory2,
                factory1
            ] = distance;

            // Console.Error.Write(
            //     "Direct distance between : "
            //         + factory1
            //         + " and "
            //         + factory2
            //         + " = "
            //         + distance
            //         + "      "
            // );
        }
        Console.Error.WriteLine();

        debugTime("After reading links");
    }

    static void CalculateClosestPaths()
    {
        WorldStatus.ClosestPathArray = new (int, List<int>)[
            WorldStatus.FactoryCount,
            WorldStatus.FactoryCount
        ];

        for (int i = 0; i < WorldStatus.FactoryCount; i++)
        for (int j = 0; j < WorldStatus.FactoryCount; j++)
        {
            WorldStatus.ClosestPathArray[i, j] = (
                (i == j) ? 0 : Gamerules.maxDistanceBetweenTwoFactories,
                new List<int>()
            );
        }

        var queue = new PriorityQueue<int, int>();

        // Dijkstra
        for (int origin = 0; origin < WorldStatus.FactoryCount; origin++)
        {
            queue.Enqueue(origin, 0);
            while (queue.UnorderedItems.Count() > 0)
            {
                queue.TryDequeue(out int currentFactoryId, out int distance);
                for (
                    int neighborFactoryId = 0;
                    neighborFactoryId < WorldStatus.FactoryCount;
                    neighborFactoryId++
                )
                {
                    if (neighborFactoryId == currentFactoryId)
                        continue;
                    int newDistance =
                        WorldStatus.ClosestPathArray[origin, currentFactoryId].Item1
                        + WorldStatus.DistanceArray[currentFactoryId, neighborFactoryId];
                    if (newDistance < WorldStatus.ClosestPathArray[origin, neighborFactoryId].Item1)
                    {
                        WorldStatus.ClosestPathArray[origin, neighborFactoryId].Item1 = newDistance;
                        WorldStatus.ClosestPathArray[origin, neighborFactoryId].Item2 = WorldStatus
                            .ClosestPathArray[origin, currentFactoryId]
                            .Item2.Append(currentFactoryId)
                            .ToList();
                        queue.Enqueue(neighborFactoryId, newDistance);
                    }
                }
            }
        }
        for (int i = 0; i < WorldStatus.FactoryCount; i++)
        {
            for (int j = 0; j < WorldStatus.FactoryCount; j++)
            {
                if (i != j)
                {
                    WorldStatus.ClosestPathArray[i, j].Item2.Add(j);
                }
            }
        }
        debugTime("After Dijkstra");
    }

    static void ReadEntities()
    {
        GameState.currentState.alliedFactories = new List<int>();
        GameState.currentState.ennemyFactories = new List<int>();
        GameState.currentState.troopsSended = new List<Troop>();
        foreach (Factory factory in GameState.currentState.Factories)
        {
            if (factory != null)
                factory.incomingTroops = new List<int>();
        }

        GameState.currentState.bombsSended.RemoveAll(bomb => bomb.id == -1);

        int entityCount = int.Parse(Console.ReadLine()); // the number of entities (e.g. factories and troops)
        resetTime();

        for (int i = 0; i < entityCount; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int entityId = int.Parse(inputs[0]);
            string entityType = inputs[1];
            int entityOwner = int.Parse(inputs[2]);
            int arg2 = int.Parse(inputs[3]);
            int arg3 = int.Parse(inputs[4]);
            int arg4 = int.Parse(inputs[5]);
            int arg5 = int.Parse(inputs[6]);
            // Console.Error.WriteLine(
            //     entityId
            //         + " "
            //         + entityType
            //         + " "
            //         + entityOwner
            //         + " "
            //         + arg2
            //         + " "
            //         + arg3
            //         + " "
            //         + arg4
            //         + " "
            //         + arg5
            // );

            if (entityType == "FACTORY")
            {
                int FactoryTroopsArg = arg2;
                int FactoryProductionArg = arg3;
                int FactoryDowntimeArg = arg4;
                //int arg5 = arg5;

                if (WorldStatus.CurrentTurn == 1)
                {
                    GameState.currentState.Factories[entityId] = new Factory(
                        entityId,
                        entityOwner,
                        FactoryTroopsArg,
                        FactoryProductionArg,
                        FactoryDowntimeArg
                    );
                }
                else
                {
                    GameState
                        .currentState.Factories[entityId]
                        .Update(
                            entityOwner,
                            FactoryTroopsArg,
                            FactoryProductionArg,
                            FactoryDowntimeArg
                        );
                }

                if (entityOwner == Gamerules.me)
                {
                    GameState.currentState.alliedFactories.Add(entityId);
                }
                else if (entityOwner == Gamerules.them)
                {
                    GameState.currentState.ennemyFactories.Add(entityId);
                }
            }
            else if (entityType == "TROOP")
            {
                int TroopsOriginArg = arg2;
                int TroopsTargetArg = arg3;
                int TroopsNumberArg = arg4;
                int TroopsDelayArg = arg5;

                GameState.currentState.troopsSended.Add(
                    new Troop(
                        entityId,
                        entityOwner,
                        TroopsOriginArg,
                        TroopsTargetArg,
                        TroopsNumberArg,
                        TroopsDelayArg
                    )
                );

                GameState.currentState.Factories[TroopsTargetArg].AddIncomingTroop(entityId);
            }
            else if (entityType == "BOMB")
            {
                int BombOriginArg = arg2;
                int BombTargetArg = arg3;
                int BombDelayArg = arg4;

                if (GameState.currentState.bombsSended.Any(bomb => bomb.id == entityId))
                    GameState
                        .currentState.bombsSended.Where(bomb => bomb.id == entityId)
                        .ToList()
                        .ForEach(p => p.Update());
                else
                {
                    if (entityOwner == Gamerules.me)
                    {
                        GameState.currentState.bombsSended.Add(
                            new Bomb(
                                entityId,
                                entityOwner,
                                BombOriginArg,
                                BombTargetArg,
                                BombDelayArg
                            )
                        );
                    }
                    else
                    {
                        foreach (Factory fact in GameState.currentState.Factories)
                        {
                            if (fact.id == BombOriginArg)
                                continue;
                            GameState.currentState.bombsSended.Add(
                                new Bomb(
                                    entityId,
                                    entityOwner,
                                    BombOriginArg,
                                    fact.id,
                                    WorldStatus.DistanceArray[BombOriginArg, fact.id]
                                )
                            );
                        }
                    }
                }
            }
        }

        // REMOVE ENNEMY BOMBS
        foreach (
            Bomb impossibleBomb in GameState.currentState.bombsSended.Where(bomb =>
                bomb.owner == Gamerules.them
                && bomb.delay
                    > WorldStatus.DistanceArray[bomb.origin, bomb.target]
                        - (WorldStatus.CurrentTurn - bomb.launchedTurn)
            )
        )
        {
            impossibleBomb.id = -1;
            impossibleBomb.delay = -1;
        }
    }

    static void Main(string[] args)
    {
        Initialisation();

        // game loop
        while (true)
        {
            ReadEntities();
            if (WorldStatus.CurrentTurn == 1)
            {
                CalculateClosestPaths();
            }
            Bot.ChooseOrders();

            debugTime("End of turn " + WorldStatus.CurrentTurn);
            WorldStatus.CurrentTurn++;
        }
    }
}
