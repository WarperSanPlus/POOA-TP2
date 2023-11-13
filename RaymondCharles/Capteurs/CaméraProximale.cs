/// (DD/MM/YYYY) AUTHOR:
/// 13/11/2023 SAMUEL GAUTHIER:
/// - Renamed constant from 'MaxDistance' to 'MAX_DISTANCE_RANGE'
/// 
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
    private const double MAX_DISTANCE_RANGE = 3.5;
    internal const char SYMBOLE = 'X';

    /// <inheritdoc/>
    public override char Symbole => SYMBOLE;

    /// <summary>
    /// <see cref="Protagoniste"/> que cette caméra supervise
    /// </summary>
    private readonly Protagoniste target;

    private readonly IAffichable menuAssocié;

    public CaméraProximale(Point position, Protagoniste protagoniste, IAffichable panneauAffichage) : base(position)
    {
        target = protagoniste;
        menuAssocié = panneauAffichage;
    }

    /// <inheritdoc/>
    public override void MouvementObservé(Carte carte) => menuAssocié.Write(carte.ToString(), GetColor);

    // Un ternaire fonctionnerait, mais j'ai choisi ça s'il y a + de checks à faire
    private System.Drawing.Color GetColor(int x, int y, char c)
    {
        System.Drawing.Color color = System.Drawing.Color.Cyan;

        if (target.Position.Distance(x, y) > MAX_DISTANCE_RANGE)
            color = DépôtFiltres.Courant().ObtenirFiltre(c);

        return color;
    }
}