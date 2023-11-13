/// (DD/MM/YYYY) AUTHOR:
/// 13/11/2023 SAMUEL GAUTHIER:
/// - Changed constant from 'DirectionChoix' to 'ChoixDirection'
///
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Changed visibility of DirecteurHumain
/// - Changed 'protagoniste.Menu.ShowText()' to 'protagoniste.Menu.Write()'

using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Interfaces;
using RaymondCharles.Lieux;
using static RaymondCharles.Interfaces.IDirecteur;

namespace RaymondCharles.Entities;

public class DirecteurHumain : IDirecteur
{
    // Comme nous ne voulons pas que les contrôles changent pendant
    // l'exécution, nous pouvons créer des constantes pour chaque contrôle
    private const ConsoleKey ChoixQuitter = ConsoleKey.Q;

    private const ConsoleKey ChoixHaux = ConsoleKey.UpArrow;
    private const ConsoleKey ChoixBas = ConsoleKey.DownArrow;
    private const ConsoleKey ChoixDroite = ConsoleKey.RightArrow;
    private const ConsoleKey ChoixGauche = ConsoleKey.LeftArrow;

    public Choix Agir(Protagoniste protagoniste, Carte carte, IAffichable? menu)
    {
        // Afficher un menu
        menu?.Write(
            $"Carte de format {carte.Largeur} x {carte.Hauteur}\nEntrez {ChoixQuitter} pour quitter\nVotre choix?",
            ConsoleColor.White
        );

        // Lis une touche au clavier
        var keyInfo = Console.ReadKey(true);

        // Retourne le Choix, dépendant de la touche
        return keyInfo.Key switch
        {
            ChoixQuitter => Choix.Quitter,
            ChoixHaux => Choix.Haut,
            ChoixBas => Choix.Bas,
            ChoixDroite => Choix.Droite,
            ChoixGauche => Choix.Gauche,
            _ => Choix.Rien,
        };
    }
}