/// (DD/MM/YYYY) AUTHOR:
/// 24/10/2023 SAMUEL GAUTHIER:
/// - Added 'origin' and 'size' computations
/// 
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Added constant 'DISPLAY_DELAY_MS'
/// 
/// 20/10/2023 SAMUEL GAUTHIER:
/// - Changed the visibility of the cursor

using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using RaymondCharles.Struct;

namespace RaymondCharles.Affichage;

public class Écran : IObservateurMouvement, IObservateurAffichage
{
    const int DISPLAY_DELAY_MS = 0;

    readonly List<PanneauAffichage> panneaux = new();
    bool newPanneauAdded = false;
    Point origin = new();
    Point size = new();

    public void MiseÀJour(PanneauAffichage panneauAffichage)
    {
        lock (panneaux)
        {
            panneaux.Add(panneauAffichage);
            newPanneauAdded = true;

            origin = CalculateOrigin();
            size = CalculateSize();
        }
    }

    private Point CalculateOrigin()
    {
        Point lowestPoint = panneaux[0].Position;

        for (int i = 1; i < panneaux.Count; ++i)
        {
            var currentPoint = panneaux[i].Position;

            if (lowestPoint.X > currentPoint.X)
                lowestPoint.X = currentPoint.X;

            if (lowestPoint.Y > currentPoint.Y)
                lowestPoint.Y = currentPoint.Y;
        }
        return lowestPoint;
    }
    private Point CalculateSize()
    {
        Point highestPoint = panneaux[0].Position + new Point(panneaux[0].Width, panneaux[0].Height);

        for (int i = 1; i < panneaux.Count; ++i)
        {
            var currentPoint = panneaux[i].Position + new Point(panneaux[i].Width, panneaux[i].Height);

            if (highestPoint.X < currentPoint.X)
                highestPoint.X = currentPoint.X;

            if (highestPoint.Y < currentPoint.Y)
                highestPoint.Y = currentPoint.Y;
        }
        return highestPoint;
    }

    public void MouvementObservé(Carte carte)
    {
        throw new NotImplementedException();
    }

    Thread? displayThread;
    bool stopDisplayThread = false;

    /// <summary>
    /// Démarre le thread chargé de rafraîchir l'affichage
    /// </summary>
    public void Démarrer()
    {
        if (displayThread != null)
            throw new ThreadAlreadyStarted();

        displayThread = new Thread(() =>
        {
            Console.CursorVisible = false;
            while (!stopDisplayThread)
            {
                // Ne pas actualiser si aucun panneaux n'a été rajouté
                if (!newPanneauAdded)
                    continue;

                lock (panneaux)
                {
                    // Actualiser tous les panneaux
                    foreach (var p in panneaux)
                        p.ShowPixels();

                    // Supprimer tous les panneaux de la file d'attente
                    panneaux.Clear();

                    newPanneauAdded = false;
                }

                Thread.Sleep(DISPLAY_DELAY_MS);
            }
            Console.CursorVisible = true;
        });

        displayThread.Start();
    }

    /// <summary>
    /// Arrête le thread chargé de rafraîchir l'affichage
    /// </summary>
    /// <returns>Un thread a été arrêté</returns>
    public bool Arrêter()
    {
        stopDisplayThread = true; // Set flag

        if (displayThread != null)
        {
            displayThread.Join(); // Wait for thread to end
            return true;
        }
        return false;
    }

    public class ThreadAlreadyStarted : Exception { }
}

/*
    Un Écran aura une position à la console, de même qu’une largeur et une hauteur, toutes calculés
    à partir des coordonnées et des dimensions des panneaux qu’il contient.
    Vous pouvez supposer qu’un Écran sera au pire aussi grand qu’une console classique.
    Un Écran s’abonnera à ses panneaux d’affichage pour être informé lorsqu’un changement
    surviendra dans au moins l’un d’entre eux. Il démarrera (méthode Démarrer) et arrêtera
    (méthode Arrêter) un Thread chargé de rafraîchir l’affichage à chaque seconde, mais seuls
    les cases de l’écran qui ont changé depuis l’affichage le plus récent devront être mis à jour.
*/