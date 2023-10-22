/// (DD/MM/YYYY) AUTHOR:
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
    private const int MOVEMENT_DELAY_MS = 1_000;

    Thread?[]? threadsOpened;
    bool requestEnd = false;

    internal void Populer(Carte carte, int amount)
    {
        List<Point> emptySpaces = carte.Trouver(Carte.Symboles.VIDE);

        int count = emptySpaces.Count;

        if (count < amount) // Regarde si le code va crash
            throw new Centrale.PlacesLibresInsuffisantesException();

        var obstacles = new Obstacle[amount];
        string letters = "abcdefghijklmnopqstuvwxyz";

        Console.Clear();

        for (int i = 0; i < amount; ++i)
        {
            var selectedItem = emptySpaces[Random.Shared.Next(0, count)];

            Console.Write("Entrer le type de l'obstacle: ");
            string type = "brownien";// Console.ReadLine() ?? string.Empty;

            obstacles[i] = new Obstacle(
                selectedItem,
                null,
                Fabriques.FabriqueDirecteur.Créer(Fabriques.FabriqueDirecteur.TraduireSorte(type)),
                letters[i % letters.Length]
            );

            carte.Installer(obstacles[i]);

            // Déplacer l'élément afin d'optimiser la suppression
            //var lastItem = emptySpaces[^1];
            //Algos.PermuterObjets(ref selectedItem, ref lastItem);
            //emptySpaces.RemoveAt(emptySpaces.Count - 1);

            --count; // Réduire le range afin de simplement delete l'array au complet
        }

        Console.Clear();

        // Equalize threads (merci mon projet C++ :D)
        // eg: 25 entities into 4 threads => (7; 6; 6; 6)
        int BASE_NUM = amount / NUM_THREADS; // Base # of entities per thread
        int MIN_INDEX = amount - BASE_NUM * NUM_THREADS; // Threads with an additional entity

        threadsOpened = new Thread?[Math.Min(amount, NUM_THREADS)]; // Don't need 4 threads if there are only 2 entities

        int threadsCount = 0;
        int nextCount = BASE_NUM;

        if (MIN_INDEX > threadsCount)
            ++nextCount;

        int elementCount = 0;

        for (int i = 0; i < amount; ++i)
        {
            ++elementCount;

            if (elementCount == nextCount)
            {
                int begin = elementCount * i;
                int end = elementCount * (i + 1) + 1;

                // THREAD
                var newThread = new Thread(() =>
                {
                    while (!requestEnd)
                    {
                        for (int i = begin; i < end; ++i)
                        {
                            var choix = obstacles[i].Agir(carte);
                            carte.Appliquer(obstacles[i], choix);
                        }
                        Thread.Sleep(MOVEMENT_DELAY_MS);
                    }
                });
                // ---

                // Ajouter le nouveau thread
                newThread.Start();
                threadsOpened[threadsCount] = newThread;
                // ---

                ++threadsCount;

                nextCount = BASE_NUM;
                if (MIN_INDEX > threadsCount)
                    ++nextCount;
            }
        }
        // ---
    }

    /// <summary>
    /// Demande l'arrêt des threads lancés par ce <see cref="Obstacleur"/>
    /// </summary>
    public void Terminer()
    {
        requestEnd = true;

        if (threadsOpened != null)
            foreach (var item in threadsOpened)
                item?.Join();
    }
}