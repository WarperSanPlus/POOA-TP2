/// (DD/MM/YYYY) AUTHOR:
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Removed IsCharPlayer()

using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using RaymondCharles.Struct;

namespace RaymondCharles.Entities.Protagonistes;

/// <summary>
/// Sorte de <see cref="Protagoniste"/>
/// </summary>
public class Participant : Protagoniste
{
    /// <summary>
    /// Symbole représentant un <see cref="Participant"/> sur la carte
    /// </summary>
    public const char SYMBOLE = 'R';

    /// <summary>
    /// Symbole représentant un <see cref="Participant"/> ayant réussi
    /// </summary>
    public const char RÉUSSITE_SYMBOLE = '!';

    /// <inheritdoc cref="SYMBOLE"/>
    public override char Symbole => SYMBOLE;

    /// <inheritdoc/>
    public Participant(Point initialPosition, IAffichable? menu) : base(initialPosition, menu) { }

    /// <inheritdoc/>
    public Participant(Point initialPosition, IAffichable? menu, IDirecteur directeur) : base(initialPosition, menu, directeur) { }

    /// <inheritdoc/>
    public override char ObtenirSymbole(Carte carte) => carte.EstSurFrontière(Position) ? RÉUSSITE_SYMBOLE : base.ObtenirSymbole(carte);
}