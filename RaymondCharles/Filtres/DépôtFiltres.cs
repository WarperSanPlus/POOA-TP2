namespace RaymondCharles.Filtres;

internal static class DépôtFiltres
{
    private static readonly Stack<FiltreCaméra> FiltresActifs = new();

    /// <summary>
    /// Ajoute <paramref name="filtre"/> au dessus de <see cref="FiltresActifs"/>
    /// </summary>
    /// <param name="filtre"></param>
    public static void Empiler(FiltreCaméra filtre) => FiltresActifs.Push(filtre);

    /// <summary>
    /// Retire le <see cref="FiltreCaméra"/> se trouvant au dessus de <see cref="FiltresActifs"/>
    /// </summary>
    public static void Dépiler() => FiltresActifs.Pop();

    /// <returns><see cref="FiltreCaméra"/> se trouvant au dessus de <see cref="FiltresActifs"/></returns>
    public static FiltreCaméra Courant() => FiltresActifs.Peek();

    /// <inheritdoc/>
    public static int Taille => FiltresActifs.Count;
}