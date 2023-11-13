/// (DD/MM/YYYY) AUTHOR:
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Removed ShouldProtagonisteContinue()

using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using static RaymondCharles.Interfaces.IDirecteur;

namespace RaymondCharles.Entities;

/// <summary>
/// Évalue l'état de l'expérimentation et détermine si celle-ci doit se poursuivre ou non
/// </summary>
public class Diagnosticien : IObservateurComportements
{
    private const int MAX_COLLISIONS = 5;
    private const string DESIRED_STOP = "Départ volontaire";
    private const string HEALTH_STOP = "Risque pour la santé du / de la participant(e)";
    private const string WIN_STOP = "Succès : le / la participant(e) a quitté la pièce";
    private const string UNKNOWN_STOP = "L'expérimentation s'est arrêtée pour une raison inconnue";

    private readonly Dictionary<Protagoniste, int> CollisionsForProtagoniste = new();
    //readonly Dictionary<Protagoniste, string> StopReasonsForProtagonistes = new();

    private string? raisonArrêt = null;

    /// <returns>L'expérimentation doit se poursuivre, considérant l'état de <paramref name="participant"/></returns>
    public bool Poursuivre(Carte carte, Participant participant) => raisonArrêt == null && !carte.EstSurFrontière(participant.Position);

    /// <inheritdoc/>
    /// Possibilité d'overflow
    ///
    /// Version étendue
    /// if (CollisionsForProtagoniste.TryGetValue(protagoniste, out int hits))
    ///    CollisionsForProtagoniste[protagoniste] = hits + 1;
    /// else
    ///     CollisionsForProtagoniste.Add(protagoniste, 1);
    public void CollisionObservée(Protagoniste protagoniste)
    {
        CollisionsForProtagoniste[protagoniste] = CollisionsForProtagoniste.TryGetValue(protagoniste, out int hits) ? hits + 1 : 1;

        if (CollisionsForProtagoniste[protagoniste] > MAX_COLLISIONS)
            raisonArrêt = HEALTH_STOP;

        Console.Beep(); // async ?
    }

    /// <returns>Explications des raisons de l'arrêt demandé par le <see cref="Diagnosticien"/></returns>
    public string ExpliquerArrêt() => raisonArrêt ?? UNKNOWN_STOP;

    /// <returns>Raison de l'arrêt causé par <paramref name="protagoniste"/></returns>
    //private string ExpliquerArrêt(Protagoniste protagoniste) => StopReasonsForProtagonistes.TryGetValue(protagoniste, out string? reason) ? reason : UNKNOWN_STOP;

    public Choix Analyser(Choix choix, Protagoniste protagoniste, Carte carte)
    {
        // En ordre de priorité
        if (choix == Choix.Quitter)
            raisonArrêt = DESIRED_STOP;
        else
        {
            bool isValid = carte.IsMovementValid(protagoniste, choix, out Struct.Point pos);

            if (isValid && carte.EstSurFrontière(pos))
                raisonArrêt = WIN_STOP;
        }
        return choix;
    }

    // Pour lorsque un diagnosticien pourra gérer plusieurs protagoniste
    //private void SetStopReason(Protagoniste protagoniste, string reason) => StopReasonsForProtagonistes[protagoniste] = reason;
}