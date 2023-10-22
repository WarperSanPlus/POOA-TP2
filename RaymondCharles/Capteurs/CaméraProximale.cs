/// (DD/MM/YYYY) AUTHOR:
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Made 'menuAssocié' read only
/// - Renamed 'Protagoniste' to 'target'

using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Filtres;
using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using RaymondCharles.Struct;

namespace RaymondCharles.Capteurs;

/// <summary>
/// <inheritdoc cref="Caméra"/>. En plus, les cases se trouvant à une distance inférieure
/// ou égale à <see cref="MaxDistance"/>
/// </summary>
internal class CaméraProximale : Capteur
{
    const double MaxDistance = 3.5;
    internal const char SYMBOLE = 'X';

    /// <inheritdoc/>
    public override char Symbole => SYMBOLE;

    /// <summary>
    /// <see cref="Protagoniste"/> que cette caméra supervise
    /// </summary>
    readonly Protagoniste target;

    readonly IAffichable menuAssocié;

    public CaméraProximale(Point position, Protagoniste protagoniste, IAffichable panneauAffichage) : base(position)
    {
        target = protagoniste;
        menuAssocié = panneauAffichage;
    }

    /// <inheritdoc/>
    public override void MouvementObservé(Carte carte)
    {
        menuAssocié.Write(carte.ToString(),
            (x, y, c) =>
            target.Position.Distance(x, y) > MaxDistance ?
            DépôtFiltres.Courant().ObtenirFiltre(c) :
            System.Drawing.Color.Cyan
        );
    }
}
