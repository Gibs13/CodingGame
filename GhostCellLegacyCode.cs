int[,] closestPathDistance; // minimal between two factories
closestPathDistance = new int[factoryCount, factoryCount];

List<int>[,] closestPath; // minimal between two factories
closestPath = new List<int>[factoryCount, factoryCount];

for (int i = 0; i < factoryCount; i++)
for (int j = 0; j < factoryCount; j++)
{
    closestPath[i, j] = new List<int>();
    if (i == j)
        continue;
    closestPathDistance[i, j] = 999;
}

var queue = new PriorityQueue<int, int>();

// Dijkstra
for (int origin = 0; origin < factoryCount; origin++)
{
    queue.Enqueue(origin, 0);
    while (queue.UnorderedItems.Count() > 0)
    {
        queue.TryDequeue(out int current, out int distance);
        for (int i = 0; i < factoryCount; i++)
        {
            int newDistance =
                closestPathDistance[origin, current] + WorldStatus.DistanceArray[origin, i];
            if (newDistance < closestPathDistance[origin, i])
            {
                closestPathDistance[origin, i] = closestPathDistance[i, origin] = newDistance;
                closestPath[origin, i].Add(current);
                closestPath[i, origin].Insert(0, current);
                queue.Enqueue(i, newDistance);
            }
        }
    }
}

debugTime("After Dijkstra");

for (int i = 0; i < factoryCount; i++)
for (int j = 0; j < factoryCount; j++)
{
    if (i == j)
        continue;
    Console.Error.Write(
        " Closest distance between : " + i + " and " + j + " = " + closestPathDistance[i, j]
    );
    foreach (int node in closestPath[i, j])
    {
        Console.Error.Write(
            " going through : " + node + " distance = " + WorldStatus.DistanceArray[i, node]
        );
    }
}
Console.Error.WriteLine();


/// strategies :
///
/// Si faire progresser la production coute cher et que la distance entre les deux bases de depart est courte - Aggressive : envoyer toutes les troupes vers les bases influentes en passant par les plus court chemins
///
/// Si faire progresser la production ne coute pas cher - Constructive : envoyer un minimum de troupes vers les bases influentes
/// pour les defendre et capturer les bases "safes" pour augmenter la production
/// defendre les troupes arrivantes et envoyer le surplus de troupes vers les bases influentes
///
/// Maximisation de la perte ennemie :
/// envoyer une bombe :
///     - sur une base qui est la cible d'une troupe ennemie nombreuse que je puisse intercepter au meme tour
///     - sur une base ennemie productive a partir d'une base proche (d'ou l'interet de recuperer les bases influentes)
///
/// Tant que l'adversaire a au moins 1 bombe, ne pas envoyer trop de troupes dans une factory qui est plus proche de l'adversaire
/// 
/// Definir
/// base influente : base centrale, qui a un plus court chemin vers les autres bases minimum
/// base safe : base qui est loin des bases ennemies
/// influence : joueur qui possede la base la plus proche, ainsi que le nombre de troupe qui peuvent la rejoindre
///
/// Quelle est la production de la premiere base ?
/// 0
/// 1
/// 2
/// 3
///
///
/// Quel est le nombre de troupes de la premiere base ?
///
///
///
/// Quelles sont les bases les plus proches et combien de troupes cela coute d'y aller ?
///
///
///
///
/// Vers quelles premieres bases choisit il de se diriger ?
///
///
///
///
/// Puis je atteindre ces bases avant qu'il ai de nouvelles troupes ?
///
///
///
///
/// Attendre de savoir ce qu'il fait si mon reward peut etre plus important
///
///
///
/// Calculer un chemin le plus court qui ne me coute pas plus d'unité que nécessaire
///



    // public static void DecideActions()
    // {
    //     int[,] orders;

    //     // Used to order best factories
    //     int[] rewards = new int[factoryCount];
    //     List<int> factoriesToSend = new List<int>();

    //     foreach (int factoryId in factoriesList)
    //     {
    //         int toSend = 0;
    //         if (GameState.currentState.Factories[factoryId].owner == Gamerules.me)
    //         {
    //             toSend = 0;
    //         }
    //         else
    //         {
    //             toSend =GameState.currentState.Factories[factoryId].troops;
    //         }

    //         foreach (int[] incomingtroop in troopsSended)
    //         {
    //             if (incomingtroop[1] == factoryId)
    //             {
    //                 if (incomingtroop[0] == Gamerules.me)
    //                     toSend -= incomingtroop[2];
    //                 else
    //                 {
    //                     toSend += incomingtroop[2];
    //                 }
    //             }
    //         }

    //         toSend += 1;

    //         if (toSend > 0)
    //             factoriesToSend.Add(factoryId);
    //         else
    //             continue;

    //         Console.Error.WriteLine(
    //             "Target Factory : " + factoryId + " with " + toSend + " troops"
    //         );

    //         // Select closest factories to send from
    //         GameState.currentState.ennemyFactories.Sort(
    //             (x, y) => x.DistanceToFactory(factoryId).CompareTo(y.DistanceToFactory(factoryId))
    //         );

    //         foreach (int myFactoryId in myFactories)
    //         {
    //             int toSendTemp = toSend;

    //             if (GameState.currentState.Factories[factoryId].owner == Gamerules.them)
    //                 toSendTemp +=
    //                    GameState.currentState.Factories[factoryId].production
    //                     * (WorldStatus.DistanceArray[myFactoryId, factoryId] + 1);

    //             int toSendFromHere =
    //                 toSendTemp >GameState.currentState.Factories[myFactoryId].troops
    //                     ?GameState.currentState.Factories[myFactoryId].troops
    //                     : toSendTemp;

    //             // Reward function
    //             int reward =
    //                 (20 - WorldStatus.DistanceArray[myFactoryId, factoryId])
    //                     *GameState.currentState.Factories[factoryId].production
    //                 - (
    //                    GameState.currentState.Factories[factoryId].owner == Gamerules.them
    //                         ? 0
    //                         : toSendFromHere
    //                 );
    //             rewards[factoryId] = toSendFromHere == toSend ? reward : rewards[factoryId];

    //             Console.Error.WriteLine(
    //                 "My factory "
    //                     + myFactoryId
    //                     + " targets "
    //                     + factoryId
    //                     + " with "
    //                     + toSendFromHere
    //                     + " troops, reward : "
    //                     + rewards[factoryId]
    //             );
    //             orders[myFactoryId, factoryId] = toSendFromHere;
    //             toSend -= toSendFromHere;
    //             if (toSend == 0)
    //                 break;
    //         }
    //         if (myFactories.Any() && toSend > 10 && bombs > 0)
    //         {
    //             orders[myFactories.First(), factoryId] = -1;
    //         }
    //     }

    //     // Write an action using Console.WriteLine()
    //     // To debug: Console.Error.WriteLine("Debug messages...");

    //     bool orderSent = false;

    //     // Sort from biggest reward
    //     factoriesToSend.Sort((x, y) => rewards[y].CompareTo(rewards[x]));

    //     foreach (int targetFactory in factoriesToSend)
    //     {
    //         if (rewards[targetFactory] < 0)
    //             continue;
    //         foreach (int originFactory in myFactories)
    //         {
    //             switch (orders[originFactory, targetFactory])
    //             {
    //                 case -1:
    //                     if (originFactory == targetFactory)
    //                         break;
    //                     if (bombs == 0)
    //                         break;
    //                     bombs--;
    //                     Console.Write(
    //                         (orderSent ? ";" : "") + "BOMB " + originFactory + " " + targetFactory
    //                     );
    //                     orderSent = true;
    //                     break;
    //                 case 0:
    //                     break;
    //                 default:
    //                     if (
    //                        GameState.currentState.Factories[originFactory].troops
    //                         >= orders[originFactory, targetFactory]
    //                     )
    //                        GameState.currentState.Factories[originFactory].troops -= orders[
    //                             originFactory,
    //                             targetFactory
    //                         ];
    //                     else
    //                         break;
    //                     Console.Error.WriteLine(
    //                         "My factory "
    //                             + originFactory
    //                             + " sends to "
    //                             + targetFactory
    //                             + " with "
    //                             + orders[originFactory, targetFactory]
    //                             + " troops"
    //                     );
    //                     if (originFactory == targetFactory)
    //                         break;
    //                     Console.Write(
    //                         (orderSent ? ";" : "")
    //                             + "MOVE "
    //                             + originFactory
    //                             + " "
    //                             + targetFactory
    //                             + " "
    //                             + orders[originFactory, targetFactory]
    //                     );
    //                     orderSent = true;
    //                     break;
    //             }
    //         }
    //     }

    //     if (orderSent)
    //         Console.WriteLine();
    //     else
    //         Console.WriteLine("WAIT");
    // }