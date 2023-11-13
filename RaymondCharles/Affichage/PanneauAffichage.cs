/// (DD/MM/YYYY) AUTHOR:
/// 13/11/2023 SAMUEL GAUTHIER:
/// - Removed Projeter()
///
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

    Color IAffichable.Fond => BackgroundColor;

    #region Observateurs Affichage

    private readonly List<IObservateurAffichage> observateursAffichage = new();

    public void Abonner(IObservateurAffichage observateurAffichage)
    {
        lock (observateurAffichage) // Waits for any iteration to end
            observateursAffichage.Add(observateurAffichage);
    }

    #endregion Observateurs Affichage

    #region IAffichable

    /// <inheritdoc cref="WindowRect.ClearText(bool)"/>
    public void Clear() => ClearText(false);

    public void Write(string message, ConsoleColor couleur)
    {
        var correctColor = couleur.ToColor(); // Prevent to call ToColor() each time
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
        colorRule ??= new Func<int, int, char, Color>((_, _, _) => this.TextColor);

        ShowText(message, colorRule, false);

        lock (observateursAffichage) // Prevent to add an observateur while iterating
            observateursAffichage.ForEach(ioa => ioa.MiseÀJour(this));
    }

    #endregion IAffichable
}