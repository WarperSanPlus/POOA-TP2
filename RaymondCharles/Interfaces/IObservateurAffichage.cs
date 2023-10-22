/// (DD/MM/YYYY) AUTHOR:
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Changed class visibility

using RaymondCharles.Affichage;

namespace RaymondCharles.Interfaces;

/// <summary>
/// Détermine les objets souhaitant être informé de changements à un <see cref="PanneauAffichage"/>
/// </summary>
public interface IObservateurAffichage
{
    /// <summary>
    /// Un changenement à été fait sur <paramref name="panneauAffichage"/>
    /// </summary>
    void MiseÀJour(PanneauAffichage panneauAffichage);
}