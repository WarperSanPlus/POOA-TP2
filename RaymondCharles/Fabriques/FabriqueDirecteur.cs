using RaymondCharles.Entities;
using RaymondCharles.Interfaces;
using static RaymondCharles.Interfaces.IDirecteur;

namespace RaymondCharles.Fabriques;

public static class FabriqueDirecteur
{
    public static SorteDirecteur TraduireSorte(string nomSorte)
        => Enum.TryParse(typeof(SorteDirecteur), nomSorte, out object? sorte) && sorte != null ? (SorteDirecteur)sorte : SorteDirecteur.inconnu;

    public static IDirecteur Créer(SorteDirecteur sorte) => sorte switch
    {
        SorteDirecteur.brownien => new DirecteurBrownien(),
        _ => new DirecteurHumain(), // inconnu & humain
    };
}