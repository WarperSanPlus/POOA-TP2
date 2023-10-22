/// (DD/MM/YYYY) AUTHOR:
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
    private const ConsoleKey QuitterChoix = ConsoleKey.Q;
    private const ConsoleKey HautChoix = ConsoleKey.UpArrow;
    private const ConsoleKey BasChoix = ConsoleKey.DownArrow;
    private const ConsoleKey DroiteChoix = ConsoleKey.RightArrow;
    private const ConsoleKey GaucheChoix = ConsoleKey.LeftArrow;

    public Choix Agir(Protagoniste protagoniste, Carte carte, IAffichable? menu)
    {
        // Afficher un menu
        menu?.Write(
            $"Carte de format {carte.Largeur} x {carte.Hauteur}\nEntrez {QuitterChoix} pour quitter\nVotre choix?",
            ConsoleColor.White
        );

        // Lis une touche au clavier
        var keyInfo = Console.ReadKey(true);

        // Retourne le Choix, dépendant de la touche
        return keyInfo.Key switch
        {
            QuitterChoix => Choix.Quitter,
            HautChoix => Choix.Haut,
            BasChoix => Choix.Bas,
            DroiteChoix => Choix.Droite,
            GaucheChoix => Choix.Gauche,
            _ => Choix.Rien,
        };
    }
}