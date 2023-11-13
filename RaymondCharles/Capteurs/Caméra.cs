/// (DD/MM/YYYY) AUTHOR:
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Made 'menuAssocié' read only

using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using RaymondCharles.Struct;

namespace RaymondCharles.Capteurs;

/// <summary>
/// Type de <see cref="Capteur"/> actualisant la <see cref="Carte"/> lorsque <see cref="MouvementObservé(Carte)"/>
/// </summary>
internal class Caméra : Capteur
{
    /// <inheritdoc/>
    public override char Symbole => 'c';

    private readonly IAffichable menuAssocié;

    public Caméra(Point position, IAffichable panneauAffichage) : base(position)
    {
        menuAssocié = panneauAffichage;
    }

    /// <inheritdoc/>
    // Update visual
    public override void MouvementObservé(Carte carte) => menuAssocié.Write(carte.ToString(), ConsoleColor.White);
}