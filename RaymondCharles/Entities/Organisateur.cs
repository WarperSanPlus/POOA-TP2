/// (DD/MM/YYYY) AUTHOR:
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Changed Organiser() to static

using RaymondCharles.Affichage;
using RaymondCharles.Capteurs;
using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Lieux;
using RaymondCharles.Static;
using static RaymondCharles.Interfaces.IDirecteur;

namespace RaymondCharles.Entities;

public class Organisateur
{
    /// <summary>
    /// Créer une <see cref="Centrale"/>, la populer, puis créer un <see cref="Diagnosticien"/>
    /// </summary>
    /// <param name="carte"></param>
    /// <param name="protagoniste"></param>
    /// <returns></returns>
    public static (Diagnosticien, Écran, Obstacleur) Organiser(
        Carte carte, 
        Protagoniste protagoniste, 
        SorteDirecteur sorte, 
        PanneauAffichage menu)
    {
        // Créer le panneau d'affichage où le jeu s'affichera
        var jeuPanneau = new PanneauAffichage(0, 0, carte.Largeur, carte.Hauteur, "GAME");

        // Créer un écran qui s'occupera d'afficher les panneaux auxquels il s'abonnera
        var écran = new Écran();

        menu.Abonner(écran);
        jeuPanneau.Abonner(écran); // Must be subscribe before first update
        
        // Mettre l'ardoise à droite du jeu
        var ardoise = new Ardoise(
            jeuPanneau.Position.X + jeuPanneau.Width + 1,
            0,
            ConsoleClassique.NB_COLONNES - jeuPanneau.Width - 1,
            carte.Hauteur, 
            "Ardoise"
        );
        ardoise.Abonner(écran);

        // Créer un obstacleur
        var obstacleur = new Obstacleur();
        obstacleur.Populer(carte, 3); // Must create obstacles before first update

        // Créer une Centrale
        var centrale = new Centrale();

        // S'assurer que la Centrale soit populée
        centrale.Populer(carte, protagoniste, jeuPanneau, ardoise, ardoise);

        // Créer un Diagnosticien
        var diagnc = new Diagnosticien();

        // L'abonner au Protagoniste
        protagoniste.Abonner(diagnc);

        // Associer un IDirecteur au protagoniste
        protagoniste.Associer(Fabriques.FabriqueDirecteur.Créer(sorte));

        // Abonner l'écran à la carte
        //carte.Abonner(écran);
        
        // Démarrer l'affichage de l'écran
        écran.Démarrer();

        // Démarrer le mouvement des obstacles
        obstacleur.Démarrer();

        // Retourner le tout
        return (diagnc, écran, obstacleur);
    }
}
