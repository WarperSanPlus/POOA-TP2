/// (DD/MM/YYYY) AUTHOR:
/// 13/11/2023 SAMUEL GAUTHIER:
/// - Implemented IDisposable
/// - Moved code from Finalizer to Dispose()
///
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Changed visibility of Symbole

using GénérateurId;
using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using RaymondCharles.Struct;

namespace RaymondCharles.Capteurs;

public abstract class Capteur : IObservateurMouvement, IDisposable
{
    private static readonly IGénérateurId GénérateurCapteur = new FabriqueGénérateurs().Créer(TypeGénérateur.Recycleur);

    /// <summary>
    /// Position du <see cref="Capteur"/>
    /// </summary>
    public Point Position { get; init; }

    private Identifiant identifiant;

    /// <summary>
    /// Symbole à afficher sur la <see cref="Carte"/>
    /// </summary>
    public abstract char Symbole { get; }

    public Capteur(Point position)
    {
        Position = position;
        identifiant = GénérateurCapteur.Prendre();
    }

    /// <inheritdoc/>
    public abstract void MouvementObservé(Carte carte);

    // Quand le capteur n'est plus utilisé, retourner l'identifiant
    public void Dispose() => GénérateurCapteur.Rendre(identifiant);
}