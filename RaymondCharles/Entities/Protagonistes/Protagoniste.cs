/// 19/10/2023 SAMUEL GAUTHIER:
/// - Reodered the class
/// - Moved Protagoniste.Choix to IDirecteur

/// (DD/MM/YYYY) AUTHOR:
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Added Associer()
/// - Added setter to Directeur
/// - Added the use of Associer() in constructor
/// - Added the use of Déplacer() in constructor
/// - Changed 'new DirecteurHumain()' to 'Fabriques.FabriqueDirecteur.Créer(SorteDirecteur.humain))'

using ArrayUtils;
using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using RaymondCharles.Struct;
using static RaymondCharles.Interfaces.IDirecteur;

namespace RaymondCharles.Entities.Protagonistes;

/// <summary>
/// Intervenant humain
/// </summary>
public abstract class Protagoniste
{
    /// <inheritdoc/>
    public Protagoniste(Point initialPosition, IAffichable? menu)
        : this(initialPosition, menu, Fabriques.FabriqueDirecteur.Créer(SorteDirecteur.humain))
    { }

    /// <inheritdoc/>
    public Protagoniste(Point initialPosition, IAffichable? menu, IDirecteur directeur)
    {
        Déplacer(initialPosition); // Dans le cas où il y a une procédure à suivre pour déplacer un protagoniste
        Menu = menu;
        Associer(directeur); // Dans le cas où il y a une procédure à suivre pour associer un directeur
    }

    #region Directeur

    /// <summary>
    /// Menu dans le directeur de ce protagoniste affichera ses informations
    /// </summary>
    private IAffichable? Menu { get; init; }

    /// <summary>
    /// Directeur contrôlant l'entité
    /// </summary>
    private IDirecteur? Directeur { get; set; }

    /// <summary>
    /// Fait agir ce <see cref="Protagoniste"/>
    /// </summary>
    public Choix Agir(Carte carte) => Directeur?.Agir(this, carte, Menu) ?? Choix.Rien;

    /// <summary>
    /// Met <paramref name="directeur"/> comme le <see cref="Directeur"/> de ce protagoniste.
    /// </summary>
    /// Comme les enfants de cette classe n'ont pas accès à Directeur, je préfère que cette classe offre le service directement
    public void Associer(IDirecteur directeur) => Directeur = directeur;

    #endregion Directeur

    #region Position

    /// <summary>
    /// Position courante du <see cref="Protagoniste"/>
    /// </summary>
    public Point Position { get; private set; }

    /// <summary>
    /// Modifie la <see cref="Position"/>
    /// </summary>
    public void Déplacer(Point position) => Position = position;

    #endregion Position

    #region IObservateurComportement

    /// <summary>
    /// Observateurs à signaler lorsqu'un comportement a été réalisé
    /// </summary>
    private readonly List<IObservateurComportements> AbonnésComportements = new();

    /// <summary>
    /// Abonne <paramref name="comportements"/> aux <see cref="IObservateurComportements"/> de cette instance
    /// </summary>
    /// Params pour permettre d'abonner plusieurs observateurs d'un coup
    public void Abonner(params IObservateurComportements[] observateurs) => observateurs.ForEach(oc => AbonnésComportements.Add(oc));

    /// <summary>
    /// Informe les abonnés qu'une collision a été observée sur ce <see cref="Protagoniste"/>
    /// </summary>
    /// La seule raison pourquoi j'ai utilisé .ForEach au lieu de for each est pour faire une méthode d'une ligne.
    /// Si nous devions rajouté quelque chose avant/après le for each, il faudrait le changer
    public void Collision() => AbonnésComportements.ForEach((item) =>
    {
        item.CollisionObservée(this);
    });

    #endregion IObservateurComportement

    #region Symbole

    /// <summary>
    /// Symbole représentant le <see cref="Protagoniste"/> sur la carte
    /// </summary>
    public abstract char Symbole { get; }

    /// <returns>Symbole à afficher selon <paramref name="carte"/></returns>
    public virtual char ObtenirSymbole(Carte carte) => Symbole;

    #endregion Symbole
}