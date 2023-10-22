using RaymondCharles.Affichage;
using RaymondCharles.Entities;
using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Fabriques;
using RaymondCharles.Lieux;
using static RaymondCharles.Interfaces.IDirecteur;

var fab = new FabriqueCarte(
    () => new List<char>() { Participant.SYMBOLE },
    () => new List<char>() { Participant.SYMBOLE },
    () => new List<char>() { Carte.Symboles.MUR, Carte.Symboles.VIDE, Participant.SYMBOLE }
);

var carte = fab.Créer(args.Length == 0 ? "../../../CarteTest.txt" : args[0]);
PanneauAffichage menu = FabriquePanneau.Créer(carte, 4, 80, "État", ConsoleColor.DarkCyan);

var raymond = new Participant(carte.Trouver(Participant.SYMBOLE)[0], menu);

SorteDirecteur sorte = args.Length >= 2 ? FabriqueDirecteur.TraduireSorte(args[1]) : SorteDirecteur.humain;
var (diag, écran, obst) = Organisateur.Organiser(carte, raymond, sorte, menu);

while (diag.Poursuivre(carte, raymond))
{
    var choix = diag.Analyser(raymond.Agir(carte), raymond, carte);
    carte.Appliquer(raymond, choix);
}
obst.Terminer();
menu.Write(diag.ExpliquerArrêt(), ConsoleColor.Yellow);
écran.Arrêter(); // arrêt de l'affichage
Console.ResetColor();