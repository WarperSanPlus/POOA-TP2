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
    private const string SEPARATOR = "| ";
    private const int MAX_LENGTH = 15;

    private readonly int drawableHeight;

    private readonly List<char> symbolesAssociated = new();
    private readonly List<int> counters = new();

    private readonly List<List<(string text, Color color)>> LinesForCapteurs = new();

    public Ardoise(int x, int y, int width, int height, string name) : base(x, y, width, height, name)
    {
        this.drawableHeight = height;
        this.FrameColor = Color.Purple;
        //this.BackgroundColor = Color.Purple;
    }

    public void Ajouter(char symbole, string message, ConsoleColor couleur)
        => Ajouter(symbole, message, couleur.ToColor());

    public void Ajouter(char symbole, string message, Color couleur)
    {
        int index = symbolesAssociated.IndexOf(symbole);

        // Build the message
        string text = BuildMessage(index != -1 ? counters[index] : 0, symbole, message);

        // Add the message to the symbole's list
        if (index != -1)
        {
            var v = LinesForCapteurs[index];
            v.Insert(0, (text, couleur));

            // Remove all lines past drawable height
            if (v.Count > drawableHeight)
                v.RemoveRange(drawableHeight, v.Count - drawableHeight);

            ++counters[index];
        }
        else
        {
            symbolesAssociated.Add(symbole);
            LinesForCapteurs.Add(new List<(string, Color)> { (text, couleur) });
            counters.Add(0);
        }

        // Write the display
        Write(BuildString(), (x, y, _) =>
        {
            int xRelative = x / MAX_LENGTH; // col #

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
            for (int j = 0; j != symbolesAssociated.Count; ++j)
            {
                result += LinesForCapteurs[j].Count <= i ?
                    new string(' ', MAX_LENGTH) :
                    LinesForCapteurs[j][i].text;
            }
            result += "\n";
        }
        return result;
    }

    private static string BuildMessage(int counter, char symbole, string text)
    {
        string message = $"{counter} {symbole} : {text}";

        // Add missing spaces
        if (message.Length < MAX_LENGTH)
            message += new string(' ', MAX_LENGTH - message.Length);

        // Truncate the message
        if (message.Length >= MAX_LENGTH - SEPARATOR.Length)
            message = message[..(MAX_LENGTH - SEPARATOR.Length)];

        return message + SEPARATOR; // 0 A : This is a text |
    }
}