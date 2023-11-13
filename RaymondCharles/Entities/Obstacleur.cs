/// (DD/MM/YYYY) AUTHOR:
/// 13/11/2023 SAMUEL GAUTHIER:
/// - Fix thread obstacle range
///
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Added Démarrer()
///
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Added const MOVEMENT_DELAY_MS

using RaymondCharles.Capteurs;
using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Lieux;
using RaymondCharles.Struct;

namespace RaymondCharles.Entities;

public class Obstacleur
{
    private const int NUM_THREADS = 4;
    private const int MOVEMENT_DELAY_MS = 1000;

    private Thread?[]? threadsOpened;
    private bool requestEnd = false;

    internal void Populer(Carte carte, int amount)
    {
        threadsOpened = CréerThreads(CréerObstacles(carte, amount), (obstacle) =>
        {
            var choix = obstacle.Agir(carte);
            carte.Appliquer(obstacle, choix);
        });
    }

    public void Démarrer()
    {
        if (threadsOpened == null)
            return;

        foreach (var item in threadsOpened)
            item?.Start();
    }

    /// <summary>
    /// Demande l'arrêt des threads lancés par ce <see cref="Obstacleur"/>
    /// </summary>
    public void Terminer()
    {
        requestEnd = true;

        if (threadsOpened == null)
            return;

        foreach (var item in threadsOpened)
            item?.Join();
    }

    private static Obstacle[] CréerObstacles(Carte carte, int amount)
    {
        List<Point> emptySpaces = carte.Trouver(Carte.Symboles.VIDE);

        if (emptySpaces.Count < amount) // Regarde si le code va crash
            throw new Centrale.PlacesLibresInsuffisantesException();

        var obstacles = new Obstacle[amount];
        string letters = "abcdefghijklmnopqstuvwxyz";

        Console.Clear();

        for (int i = 0; i < amount; ++i)
        {
            var rdmIndex = Random.Shared.Next(0, emptySpaces.Count);
            var selectedItem = emptySpaces[rdmIndex];

            Console.Write("Entrer le type de l'obstacle: ");
            string type = "brownien";// Console.ReadLine() ?? string.Empty;

            obstacles[i] = new Obstacle(
                selectedItem,
                null,
                Fabriques.FabriqueDirecteur.Créer(Fabriques.FabriqueDirecteur.TraduireSorte(type)),
                letters[i % letters.Length]
            );

            carte.Installer(obstacles[i]);

            emptySpaces[rdmIndex] = emptySpaces[^1];
        }

        Console.Clear();
        return obstacles;
    }
    private Thread?[] CréerThreads<T>(T[] objects, Action<T> objectUpdate)
    {
        // # of entities to distribute
        int amount = objects.Length;

        // Base # of entities per thread
        int BASE_NUM = amount / NUM_THREADS;

        // Threads with an additional entity
        int MIN_INDEX = amount - BASE_NUM * NUM_THREADS; 

        // Don't need 4 threads if there are only 2 entities
        Thread?[] threadsOpened = new Thread?[Math.Min(amount, NUM_THREADS)]; 

        // # of threads opened
        int threadsCount = 0;

        // # of entities the next thread will have
        int nextCount = BASE_NUM;

        if (MIN_INDEX > threadsCount)
            ++nextCount;

        // Current # of entities in the current thread
        int elementCount = 0;

        // Start index of the current range
        int beginIndex = 0;

        // eg: 25 entities into 4 threads => (7; 6; 6; 6)
        for (int i = 0; i < amount; ++i)
        {
            ++elementCount;

            if (elementCount != nextCount)
                continue;

            int begin = beginIndex;
            int end = Math.Min(amount, begin + elementCount);

            threadsOpened[threadsCount] = new Thread(() =>
            {
                while (!requestEnd)
                {
                    if (objectUpdate != null)
                    {
                        for (int j = begin; j < end; ++j)
                            objectUpdate.Invoke(objects[j]);
                    }
                    Thread.Sleep(MOVEMENT_DELAY_MS);
                }
            });
            // ---

            ++threadsCount;

            nextCount = BASE_NUM;
            if (MIN_INDEX > threadsCount)
                ++nextCount;

            beginIndex += elementCount;
            elementCount = 0;
        }
        return threadsOpened;
    }
}