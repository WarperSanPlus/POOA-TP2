/// (DD/MM/YYYY) AUTHOR:
/// 20/10/2023 SAMUEL GAUTHIER:
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
    const int DISPLAY_DELAY_MS = 1_000;

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
        }
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
                if (!newPanneauAdded)
                    continue;

                lock (panneaux)
                {
                    for (int i = panneaux.Count - 1; i >= 0; --i)
                    {
                        panneaux[i].ShowPixels();
                        panneaux.RemoveAt(i);
                    }

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