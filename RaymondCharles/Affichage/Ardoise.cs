/// (DD/MM/YYYY) AUTHOR:
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Made 'drawableHeight' read only
/// - Fixed message building process
/// - Fixed message truncating process

using ColorUtils;
using System.Drawing;

namespace RaymondCharles.Affichage;

internal class Ardoise : PanneauAffichage
{
    const string SEPARATOR = "| ";
    const int MAX_LENGTH = 15;

    readonly int drawableHeight = 0;

    readonly Dictionary<char, int> indexesForCapteurs = new();
    readonly List<List<(string text, Color color)>> LinesForCapteurs = new();

    public Ardoise(int x, int y, int width, int height, string name) : base(x, y, width, height, name)
    {
        this.drawableHeight = height;
        this.FrameColor = Color.Purple;
        //this.BackgroundColor = Color.Purple;
    }

    public void Ajouter(char symbole, string message, ConsoleColor couleur) => Ajouter(symbole, message, couleur.ToColor());
    public void Ajouter(char symbole, string message, Color couleur)
    {
        // Build the message
        string text = $"0 {symbole} : {message}";

        if (text.Length < MAX_LENGTH)
            text += new string(' ', MAX_LENGTH - text.Length); // Add missing spaces

        // Truncate the message
        if (text.Length >= MAX_LENGTH - SEPARATOR.Length)
            text = text[..(MAX_LENGTH - SEPARATOR.Length)];
        text += SEPARATOR;

        // Add the message to the symbole's list
        if (indexesForCapteurs.TryGetValue(symbole, out int index))
        {
            var v = LinesForCapteurs[index];
            v.Insert(0, (text, couleur));

            if (v.Count > drawableHeight)
                v.RemoveRange(drawableHeight, v.Count - drawableHeight);
        }
        else
        {
            indexesForCapteurs.Add(symbole, LinesForCapteurs.Count);
            LinesForCapteurs.Add(new List<(string, Color)> { (text, couleur) });
        }

        // Write the display
        Write(BuildString(), (x, y, _) =>
        {
            int xRelative = x / MAX_LENGTH;

            // If the character is outside the ranges
            if (LinesForCapteurs.Count <= xRelative || LinesForCapteurs[xRelative].Count <= y)
                return Color.White;

            // Make the separator white
            if (x % MAX_LENGTH == MAX_LENGTH - SEPARATOR.Length)
                return Color.White;

            // Use the color of the line
            return LinesForCapteurs[xRelative][y].color;
        });
    }

    // A1 | B1 | C1
    // A2 | B2 | C2
    // A2 | B2 | C3
    private string BuildString()
    {
        string result = "";

        for (int i = 0; i != drawableHeight; ++i)
        {
            foreach (var (_, index) in indexesForCapteurs)
            {
                result += LinesForCapteurs[index].Count <= i ?
                    new string(' ', MAX_LENGTH) :
                    LinesForCapteurs[index][i].text;
            }
            result += "\n";
        }
        return result;
    }
}