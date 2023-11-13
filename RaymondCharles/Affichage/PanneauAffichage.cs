/// (DD/MM/YYYY) AUTHOR:
/// 19/10/2023 SAMUEL GAUTHIER:
/// - Made 'colorRule' in Write() nullable

/// 18/10/2023 SAMUEL GAUTHIER:
/// - Removed 'Modifié' (use 'NeedsUpdate' instead)
/// - Removed 'Hauteur' (use 'Height' instead)
/// - Removed 'Largeur' (use 'Width' instead)
/// - Removed 'Fond' (use 'BackgroundColor' instead)

using ColorUtils;
using RaymondCharles.Interfaces;
using System.Drawing;
using static ConsoleUtils.ConsoleUI;

namespace RaymondCharles.Affichage;

/// Implémenté par <see cref="WindowRect"/>
public class PanneauAffichage : WindowRect, IAffichable
{
    /// <inheritdoc/>
    public PanneauAffichage(int x, int y, int width, int height, string name) : base(x, y, width, height, name) { }

    /// <inheritdoc cref="WindowRect.Position"/>
    /// Comme WindowRect a déjà une propriété Position avec un différent type, je masque la propriété de base
    /// afin d'obtenir la position dans le bon type
    public new Struct.Point Position => new(base.Position.X, base.Position.Y);

    /// Implémenter par WindowRect.ShowText en passant 'true' pour le paramètre 'refresh'
    //public void Projeter() => this.ShowPixels();

    Color IAffichable.Fond => BackgroundColor;

    /// <inheritdoc cref="WindowRect.ClearText(bool)"/>
    public void Clear() => ClearText(false);

    private readonly List<IObservateurAffichage> observateurAffichages = new();

    public void Abonner(IObservateurAffichage observateurAffichage) => observateurAffichages.Add(observateurAffichage);

    public void Write(string message, ConsoleColor couleur)
    {
        var correctColor = couleur.ToColor();
        Write(message, (_, _, _) => correctColor);
    }

    public void Write(List<(char ch, ConsoleColor c)> message)
    {
        string text = "";

        foreach (var (character, color) in message)
            text += ColorConsole.ForeColor(color.ToColor(), character.ToString());

        ShowText(text, false);
    }

    public void Write(string message, Func<int, int, char, Color>? colorRule)
    {
        ShowText(message, colorRule, false);
        observateurAffichages.ForEach(ioa => ioa.MiseÀJour(this));
    }
}