using ColorUtils;
using System.Drawing;

namespace RaymondCharles.Filtres;

/// <summary>
/// Permet d'associer une couleur d'affichage à un symbole
/// </summary>
/// Comme WindowRect supporte Color, les couleurs sont stockées comme Color et non ConsoleColor
internal class FiltreCaméra
{
    internal enum RésultatInsertion
    { Insertion, MiseÀJour }

    private readonly Dictionary<char, Color> symbolForColor = new();

    /// <summary>
    /// Associe <paramref name="color"/> à <paramref name="symbole"/>
    /// </summary>
    public RésultatInsertion InsérerFiltre(char symbole, ConsoleColor color)
    {
        var result = symbolForColor.ContainsKey(symbole) ? RésultatInsertion.MiseÀJour : RésultatInsertion.Insertion;

        symbolForColor[symbole] = color.ToColor();

        return result;
    }

    /// <returns>La couleur associée avec <paramref name="symbole"/></returns>
    public Color ObtenirFiltre(char symbole) => symbolForColor.TryGetValue(symbole, out Color color) ? color : Color.White;
}