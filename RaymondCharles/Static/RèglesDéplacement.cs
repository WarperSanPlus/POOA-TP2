/// (DD/MM/YYYY) AUTHOR:
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Using Interfaces.Choix instead of local Choix
/// - Added constant MOVEMENT_DISTANCE
/// - Added ProchainePosition()
/// - Added CalculateMovement()
/// - Removed ChoixInvalideException

using RaymondCharles.Struct;
using static RaymondCharles.Interfaces.IDirecteur;

namespace RaymondCharles.Static;

public static class RèglesDéplacement
{
    public const int MOVEMENT_DISTANCE = 1;

    /// <returns>Position après le mouvement déterminé par <paramref name="choix"/></returns>
    /// <remarks>Il est possible que le point retourné soit hors de la carte.</remarks>
    public static Point ProchainePosition(Point pos, Choix choix) => pos + CalculateMovement(choix);

    /// <returns>Vecteur de déplacement déterminé par <paramref name="choix"/> avec une magnitude de <paramref name="distance"/>.</returns>
    private static Point CalculateMovement(Choix choix, int distance = MOVEMENT_DISTANCE) => choix switch
    {
        Choix.Droite => new Point(distance, 0),
        Choix.Bas => new Point(0, distance),
        Choix.Gauche => new Point(-distance, 0),
        Choix.Haut => new Point(0, -distance),
        _ => new Point(0, 0),
    };
}

//public class ChoixInvalideException : Exception { }