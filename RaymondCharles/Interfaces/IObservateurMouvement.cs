using RaymondCharles.Lieux;

namespace RaymondCharles.Interfaces;

/// <summary>
/// Permet à un objet d'être informé de changements de position d'objets sur une Carte
/// </summary>
public interface IObservateurMouvement
{
    /// <summary>
    /// Appeler quand une entité bouge sur <paramref name="carte"/>
    /// </summary>
    /// <param name="carte"></param>
    void MouvementObservé(Carte carte);
}