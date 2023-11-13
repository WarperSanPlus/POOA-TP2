/// (DD/MM/YYYY) AUTHOR:
/// 13/11/2023 SAMUEL GAUTHIER:
/// - Changed constants from 'REASON_STOP' to 'STOP_REASON'
/// 
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
    #region Constantes

    private const int MAX_COLLISIONS = 5;
    private const string STOP_DESIRED = "Départ volontaire";
    private const string STOP_HEALTH = "Risque pour la santé du / de la participant(e)";
    private const string STOP_WIN = "Succès : le / la participant(e) a quitté la pièce";
    private const string STOP_UNKNOWN = "L'expérimentation s'est arrêtée pour une raison inconnue";

    #endregion Constantes

    private readonly Dictionary<Protagoniste, int> collisionsForProtagoniste = new();
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
        collisionsForProtagoniste[protagoniste] = collisionsForProtagoniste.TryGetValue(protagoniste, out int hits) ? hits + 1 : 1;

        if (collisionsForProtagoniste[protagoniste] > MAX_COLLISIONS)
            raisonArrêt = STOP_HEALTH;

        Console.Beep(); // async ?
    }

    /// <returns>Explications des raisons de l'arrêt demandé par le <see cref="Diagnosticien"/></returns>
    public string ExpliquerArrêt() => raisonArrêt ?? STOP_UNKNOWN;

    /// <returns>Raison de l'arrêt causé par <paramref name="protagoniste"/></returns>
    //private string ExpliquerArrêt(Protagoniste protagoniste) => StopReasonsForProtagonistes.TryGetValue(protagoniste, out string? reason) ? reason : UNKNOWN_STOP;

    public Choix Analyser(Choix choix, Protagoniste protagoniste, Carte carte)
    {
        // En ordre de priorité
        if (choix == Choix.Quitter)
            raisonArrêt = STOP_DESIRED;
        else
        {
            bool isValid = carte.IsMovementValid(protagoniste, choix, out Struct.Point pos);

            if (isValid && carte.EstSurFrontière(pos))
                raisonArrêt = STOP_WIN;
        }
        return choix;
    }

    // Pour lorsque un diagnosticien pourra gérer plusieurs protagoniste
    //private void SetStopReason(Protagoniste protagoniste, string reason) => StopReasonsForProtagonistes[protagoniste] = reason;
}