/// (DD/MM/YYYY) AUTHOR:
/// 20/10/2023 SAMUEL GAUTHIER:
/// - Changed 'Fond' from ConsoleColor to Color

using System.Drawing;

namespace RaymondCharles.Interfaces;

public interface IAffichable
{
    Color Fond { get; }

    /// <summary>
    /// Remplace le text affichable par <paramref name="message"/> tout en suivant le code de couleur <paramref name="colorRule"/>
    /// </summary>
    void Write(string message, Func<int, int, char, Color> colorRule);

    /// <summary>
    /// Remplace le texte affichable par <paramref name="message"/> avec <paramref name="couleur"/> comme couleur d'avant-plan.
    /// </summary>
    void Write(string message, ConsoleColor couleur);

    /// <summary>
    /// Remplace le texte affichable par les caractères de cette collection, avec la bonne couleur d’avant-plan pour chaque caractère.
    /// </summary>
    void Write(List<(char ch, ConsoleColor c)> message);

    /// <summary>
    /// Remplace le contenu affichable par des espaces de la couleur du Fond
    /// </summary>
    void Clear();
}