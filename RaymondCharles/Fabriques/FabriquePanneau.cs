using System.Drawing;
using ColorUtils;
using RaymondCharles.Affichage;
using RaymondCharles.Lieux;

namespace RaymondCharles.Fabriques;

public static class FabriquePanneau
{
    public static PanneauAffichage Créer(Carte carte, int hauteur, int largeur, string nom, ConsoleColor couleurDuCadre)
        => Créer(carte, hauteur, largeur, nom, couleurDuCadre.ToColor());

    public static PanneauAffichage Créer(Carte carte, int hauteur, int largeur, string nom, Color couleurDuCadre)
    {
        // Hauteur de la map + 2 (border) + 1
        return new PanneauAffichage(0, carte.Hauteur + 2 + 1, largeur, hauteur, nom)
        {
            FrameColor = couleurDuCadre
        };
    }
}