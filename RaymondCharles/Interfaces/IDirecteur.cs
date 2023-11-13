/// (DD/MM/YYYY) AUTHOR:
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Moved Interfaces.SorteDirecteur to IDirecteur.SorteDirecteur
/// - Moved Interfaces.Choix to IDirecteur.Choix
///
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Added Choix

using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Lieux;

namespace RaymondCharles.Interfaces;

/// <summary>
/// Modélise les décisions d'un <see cref="Protagoniste"/> lorsque ce dernier agit
/// </summary>
public interface IDirecteur
{
    /// <summary>
    /// Appelé quand <paramref name="protagoniste"/> a besoin d'agir
    /// </summary>
    /// <param name="protagoniste"><see cref="Protagoniste"/> à diriger</param>
    /// <param name="carte"><see cref="Carte"/> sur laquelle se trouev <paramref name="protagoniste"/></param>
    /// <returns>Décision prise par <paramref name="protagoniste"/> en agissant</returns>
    Choix Agir(Protagoniste protagoniste, Carte carte, IAffichable? menu);

    public enum SorteDirecteur
    { inconnu, humain, brownien }

    public enum Choix
    { Droite, Haut, Gauche, Bas, Quitter, Rien }
}