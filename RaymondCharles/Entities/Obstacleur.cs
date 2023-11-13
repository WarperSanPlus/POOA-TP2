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

        #region Equalize threads (merci mon projet C++ :D)

        // eg: 25 entities into 4 threads => (7; 6; 6; 6)
        int BASE_NUM = amount / NUM_THREADS; // Base # of entities per thread
        int MIN_INDEX = amount - BASE_NUM * NUM_THREADS; // Threads with an additional entity

        threadsOpened = new Thread?[Math.Min(amount, NUM_THREADS)]; // Don't need 4 threads if there are only 2 entities

        int threadsCount = 0;
        int nextCount = BASE_NUM;

        if (MIN_INDEX > threadsCount)
            ++nextCount;

        int elementCount = 0;
        int beginIndex = 0;

        for (int i = 0; i < amount; ++i)
        {
            ++elementCount;

            if (elementCount != nextCount)
                continue;

            int begin = beginIndex;
            int end = Math.Min(obstacles.Length, begin + elementCount);

            // THREAD
            var newThread = new Thread(() =>
            {
                while (!requestEnd)
                {
                    for (int j = begin; j < end; ++j)
                    {
                        var choix = obstacles[j].Agir(carte);
                        carte.Appliquer(obstacles[j], choix);
                    }
                    Thread.Sleep(MOVEMENT_DELAY_MS);
                }
            });
            // ---

            // Ajouter le nouveau thread
            threadsOpened[threadsCount] = newThread;
            // ---

            ++threadsCount;

            nextCount = BASE_NUM;
            if (MIN_INDEX > threadsCount)
                ++nextCount;
            beginIndex += elementCount;
            elementCount = 0;
        }

        #endregion Equalize threads (merci mon projet C++ :D)
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

        if (threadsOpened != null)
            foreach (var item in threadsOpened)
                item?.Join();
    }
}