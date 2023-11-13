using RaymondCharles.Entities.Protagonistes;

namespace RaymondCharles.Interfaces;

/// <summary>
/// Permet à un objet intéressé d’être informé de certains aspects du comportement d’un <see cref="Protagoniste"/>.
/// </summary>
public interface IObservateurComportements
{
    /// <summary>
    /// Appelée lorsqu’une collision survient sur <paramref name="protagoniste"/>
    /// </summary>
    /// <param name="protagoniste"></param>
    void CollisionObservée(Protagoniste protagoniste);
}