using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using RaymondCharles.Static;
using RaymondCharles.Struct;
using static RaymondCharles.Interfaces.IDirecteur;

namespace RaymondCharles.Entities;

public class DirecteurBrownien : IDirecteur
{
    /// <summary>
    /// Déplace <paramref name="protagoniste"/> à une case adjacente
    /// </summary>
    /// <returns></returns>
    public Choix Agir(Protagoniste protagoniste, Carte carte, IAffichable? menu)
    {
        // Fetch all valid directions
        Point pos = protagoniste.Position;

        var choix = new Choix[]
        {
            Choix.Haut,
            Choix.Gauche,
            Choix.Droite,
            Choix.Bas,
        };

        var validPos = new List<Choix>();

        foreach (var item in choix)
        {
            var newPos = RèglesDéplacement.ProchainePosition(pos, item);

            if (carte.EstDisponible(newPos))
                validPos.Add(item);
        }
        // ---

        // Select one of the valid directions
        return validPos.Count != 0 ? validPos[Random.Shared.Next(0, validPos.Count)] : Choix.Rien;
    }
}