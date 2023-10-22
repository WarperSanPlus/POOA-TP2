/// (DD/MM/YYYY) AUTHOR:
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Added 'paramName' argument in the call of ArgumentException
/// 
/// 20/10/2023 SAMUEL GAUTHIER:
/// - Added static SYMBOLE ('m')

using ArrayUtils;
using RaymondCharles.Affichage;
using RaymondCharles.Lieux;
using RaymondCharles.Struct;

namespace RaymondCharles.Capteurs;

internal class DétecteurMouvement : Capteur
{
    public static char SYMBOLE = 'm';

    const double TOTAL_RANGE = 5;
    const double DETECTED_RANGE = 3.5;
    readonly Ardoise panneau;

    public override char Symbole => SYMBOLE;

    public DétecteurMouvement(Point position, PanneauAffichage panneau) : base(position)
    {
        if (panneau is not Ardoise ardoise)
            throw new ArgumentException(null, nameof(panneau));

        this.panneau = ardoise;
    }

    char[,]? lastPic = null;

    public override void MouvementObservé(Carte carte)
    {
        char[,] newPic = carte.Snapshot();

        if (lastPic == null) // Skip first mouvement (TO IMPROVE)
        {
            lastPic = newPic;
            return;
        }

        newPic.ForEach((x, y, c) =>
        {
            if (c != Carte.Symboles.VIDE && c != lastPic[x, y])
            {
                var distance = this.Position.Distance(y, x);

                if (distance > TOTAL_RANGE)
                    return;

                panneau.Ajouter(c, Math.Round(distance, 2).ToString(), distance <= DETECTED_RANGE ? ConsoleColor.Red : ConsoleColor.Yellow);
            }
        });

        lastPic = newPic;
    }
}