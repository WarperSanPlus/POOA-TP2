/// (DD/MM/YYYY) AUTHOR:
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
    const int START_CAPTEURS_COUNT = 2;
    readonly List<Capteur> CapteursInstallés = new();

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
        if (START_CAPTEURS_COUNT > panneauAffichages.Length)
            throw new IAffichablesFournisInsuffisantsException();

        var emptySpots = carte.Trouver(Carte.Symboles.VIDE);
        int count = emptySpots.Count;

        if (count < START_CAPTEURS_COUNT)
            throw new PlacesLibresInsuffisantesException();

        var capteurs = new List<Capteur>();

        //var rng = new Random();

        for (int i = 0; i < START_CAPTEURS_COUNT; ++i)
        {
            var rdmIndex = Random.Shared.Next(count);
            var pointA = emptySpots[rdmIndex];

            Capteur? cap = null;

            if (i == 0)
                cap = new CaméraProximale(pointA, protagoniste, panneauAffichages[i]);
            else
                cap = new DétecteurMouvement(pointA, panneauAffichages[i]);

            emptySpots[rdmIndex] = emptySpots[count - 1]; // Duplicates but doesn't matter

            if (cap != null)
            {
                capteurs.Add(cap);
                CapteursInstallés.Add(cap);
            }

            //emptySpots.RemoveAt(emptySpots.Count - 1);

            --count;

            if (count == 0)
                break;
        }

        carte.Installer(capteurs);
    }

    #region Exceptions
    public class PlacesLibresInsuffisantesException : Exception { }
    public class IAffichablesFournisInsuffisantsException : Exception { }
    #endregion
}
