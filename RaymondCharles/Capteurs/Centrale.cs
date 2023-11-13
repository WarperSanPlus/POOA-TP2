/// (DD/MM/YYYY) AUTHOR:
/// 13/11/2023 SAMUEL GAUTHIER:
/// - Changed Random object (global to local)
///
/// 20/10/2023 SAMUEL GAUTHIER:
/// - Changed amount of Capteur from constant to parameters amount
///
/// 20/10/2023 SAMUEL GAUTHIER:
/// - Fix random available tile picker
///
/// 19/10/2023 SAMUEL GAUTHIER:
/// - Changed IAffichable[] to PanneauAffichage[]
///
/// 18/10/2023 SAMUEL GAUTHIER:
/// - Changed CartePleinException => PlacesLibresInsuffisantesException
/// - Changed PanneauAffichage[] to IAffichable[]
/// - Added PanneauAffichage[] parameter to Populer()

using RaymondCharles.Affichage;
using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Filtres;
using RaymondCharles.Lieux;

namespace RaymondCharles.Capteurs;

/// <summary>
/// Lieu prenant en charge l'installation des instances de <see cref="Capteur"/>
/// </summary>
internal class Centrale
{
    private readonly List<Capteur> CapteursInstallés = new();

    public Centrale()
    {
        // S'assurer que le dépôt ne soit pas vide
        if (DépôtFiltres.Taille == 0)
            DépôtFiltres.Empiler(new FiltreCaméra());

        DépôtFiltres.Courant().InsérerFiltre(CaméraProximale.SYMBOLE, ConsoleColor.Red);
        DépôtFiltres.Courant().InsérerFiltre(DétecteurMouvement.SYMBOLE, ConsoleColor.Red);
    }

    internal void Populer(Carte carte, Protagoniste protagoniste, params PanneauAffichage[] panneauAffichages)
    {
        var emptySpots = carte.Trouver(Carte.Symboles.VIDE);

        if (emptySpots.Count < panneauAffichages.Length)
            throw new PlacesLibresInsuffisantesException();

        var capteurs = new List<Capteur>();

        var rng = new Random();

        for (int i = 0; i < panneauAffichages.Length; ++i)
        {
            var rdmIndex = rng.Next(emptySpots.Count);
            var pointA = emptySpots[rdmIndex];

            Capteur? cap = i == 0 ?
                new CaméraProximale(pointA, protagoniste, panneauAffichages[i]) :
                new DétecteurMouvement(pointA, panneauAffichages[i]);

            emptySpots[rdmIndex] = emptySpots[^1]; // Duplicates but doesn't matter

            if (cap != null)
            {
                capteurs.Add(cap);
                CapteursInstallés.Add(cap);
            }

            emptySpots.RemoveAt(emptySpots.Count - 1);

            if (emptySpots.Count == 0)
                break;
        }

        carte.Installer(capteurs);
    }

    #region Exceptions

    public class PlacesLibresInsuffisantesException : Exception
    { }

    #endregion Exceptions
}