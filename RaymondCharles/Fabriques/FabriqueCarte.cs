/// (DD/MM/YYYY) AUTHOR:
/// 19/10/2023 SAMUEL GAUTHIER:
/// - Made 'symbolesParticipants' read only
/// - Made 'symbolesRequis' read only
/// - Made 'symbolesPermis' read only
/// 
/// 19/10/2023 SAMUEL GAUTHIER:
/// - Added 'SymboleRequisNonTrouvéException'
/// 
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Changed exceptions to have Carte's exceptions
/// 

using ArrayUtils;
using RaymondCharles.Lieux;
using RaymondCharles.Static;

namespace RaymondCharles.Fabriques;

/// <summary>
/// Fabrique de lieu de l'expérimentation
/// </summary>
public class FabriqueCarte
{
    readonly List<char> symbolesParticipants;
    readonly List<char> symbolesRequis;
    readonly List<char> symbolesPermis;

    /// <param name="symbolesParticipants">Liste des symboles possibles pour un participant</param>
    /// <param name="symbolesRequis">Liste de symboles requis</param>
    /// <param name="symbolesPermis">Liste de symboles permis</param>
    public FabriqueCarte(Func<List<char>> symbolesParticipants, Func<List<char>> symbolesRequis, Func<List<char>> symbolesPermis)
    {
        this.symbolesParticipants = symbolesParticipants?.Invoke() ?? new();
        this.symbolesRequis = symbolesRequis?.Invoke() ?? new();
        this.symbolesPermis = symbolesPermis?.Invoke() ?? new();
    }

    /// <returns>Carte pleinement construite et validée</returns>
    /// <exception cref="Carte.PasDeCarteException"></exception>
    /// <exception cref="EmptyLineException"></exception>
    /// <exception cref="Carte.CarteNonRectangulaireException"></exception>
    public Carte Créer(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        using var reader = new StreamReader(path);

        var lines = new List<string>();
#nullable disable
        for (string s = reader.ReadLine(); s != null; s = reader.ReadLine())
            lines.Add(s);
#nullable restore

        return Valider(new Carte(ToData(Valider(lines))));
    }

    /// <summary>
    /// Vérifie que <paramref name="lines"/> peut former une <see cref="Carte"/> avec une forme valide
    /// </summary>
    private static List<string> Valider(List<string> lines)
    {
        // Check if the map is empty
        if (lines.Count == 0)
            throw new Carte.PasDeCarteException();

        int width = lines[0].Length;

        lines.ForEach(line =>
        {
            // Check if the line is empty
            if (line.Trim().Length == 0)
                throw new EmptyLineException();

            // Check if the map is a full rectangle
            if (line.Length != width)
                throw new Carte.CarteNonRectangulaireException();
        });

        return lines;
    }

    /// <returns>Représentation en tableau 2D de char de la surface décrite par <paramref name="lines"/></returns>
    private static char[,] ToData(List<string> lines)
    {
        var charArray = new char[lines.Count, lines[0].Length];

        // Fetch chaque line puis fetch chaque caractère du string
        lines.ForEach((y, line) =>
        {
            for (int x = 0; x != line.Length; ++x)
            {
                charArray[y, x] = line[x];
            }
        });
        return charArray;
    }

    public Carte Valider(Carte carte)
    {
        bool participantFound = false;

        var symbolesRequisTrouvés = new List<char>(symbolesRequis);

        for (int y = 0; y < carte.Hauteur; y++)
        {
            for (int x = 0; x < carte.Largeur; x++)
            {
                var c = carte[x, y];

                if (!symbolesPermis.Contains(c))
                    throw new Carte.SymboleIllégalException();

                if (symbolesParticipants.Contains(c))
                {
                    if (participantFound)
                        throw new ParticipantAlreadyFoundException();

                    participantFound = true;
                }

                int symboleRequisIndex = symbolesRequisTrouvés.IndexOf(c);

                if (symboleRequisIndex != -1)
                {
                    var toDelete = symbolesRequisTrouvés[symboleRequisIndex];
                    var toSwitch = symbolesRequisTrouvés[^1];

                    Algos.PermuterObjets(ref toDelete, ref toSwitch);
                    symbolesRequisTrouvés.RemoveAt(symbolesRequisTrouvés.Count - 1);
                }
            }
        }

        if (symbolesRequisTrouvés.Count != 0)
            throw new SymboleRequisNonTrouvéException();

        if (!participantFound)
            throw new Carte.SymboleManquantException();

        return carte;
    }

    #region Exceptions
    public class EmptyLineException : Exception { }
    public class ParticipantAlreadyFoundException : Exception { }
    public class SymboleRequisNonTrouvéException : Exception { }
    #endregion
}