/// (DD/MM/YYYY) AUTHOR:
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Added Symboles.BONHEUR
/// - Removed AppliquerTests()

/// 18/10/2023 SAMUEL GAUTHIER:
/// - Removed IsCharValid()
/// - Removed constant MOVEMENT_DISTANCE
/// - Moved CalculateMovement to RèglesDéplacements
/// - Changed visibility of ChangementObservé()

using RaymondCharles.Capteurs;
using RaymondCharles.Entities.Protagonistes;
using RaymondCharles.Interfaces;
using RaymondCharles.Static;
using RaymondCharles.Struct;
using static RaymondCharles.Interfaces.IDirecteur;

namespace RaymondCharles.Lieux;

/// <summary>
/// Plateau d'expérimentation
/// </summary>
public class Carte
{
    public class Symboles
    {
        public const char VIDE = ' ';
        public const char MUR = '#';
        public const char BONHEUR = Participant.RÉUSSITE_SYMBOLE;
    }

    #region Map

    /// <summary>
    /// La hauteur de la Carte en nombre de cases
    /// </summary>
    public int Hauteur => Map.GetLength(0);

    /// <summary>
    /// La largeur de la Carte en nombre de cases
    /// </summary>
    public int Largeur => Map.GetLength(1);

    /// <summary>
    /// Représentation graphique de Carte
    /// </summary>
    private readonly char[,] Map;

    public char this[int x, int y]
    {
        get => Map[y, x];
        private set => Map[y, x] = value;
    }

    public char this[Point position]
    {
        get => this[position.X, position.Y];
        private set => this[position.X, position.Y] = value;
    }

    #endregion Map

    internal Carte(char[,] map)
    {
        Map = map;
    }

    #region Predicat
    /// <returns><paramref name="position"/> est libre</returns>
    public bool EstDisponible(Point position)
    {
        // Si la position est hors de la map, pas besoin d'attendre pour le lock
        if (!EstDans(position)) 
            return false;

        lock (this) // Merci pour la DLL
            return this[position] == Symboles.VIDE;
    }

    /// <returns><paramref name="position"/> est dans la Carte</returns>
    public bool EstDans(Point position) => position.X.EstDansBornes(0, Largeur - 1) && position.Y.EstDansBornes(0, Hauteur - 1);

    /// <returns><paramref name="position"/> est sur la frontière de la Carte</returns>
    public bool EstSurFrontière(Point position) => position.X == 0 || position.X == Largeur - 1 || position.Y == 0 || position.Y == Hauteur - 1;

    #endregion Predicat

    #region Capteurs
    /// <summary>
    /// Positionne chacun des capteurs sur la Carte
    /// </summary>
    /// <param name="capteurs"></param>
    public void Installer(List<Capteur> capteurs)
    {
        lock (this) // Merci pour la DLL
        {
            capteurs.ForEach((capteur) =>
            {
                if (!EstDisponible(capteur.Position))
                    throw new PositionIllégaleException();
                    //return;

                this[capteur.Position] = capteur.Symbole;
                Abonner(capteur);
            });
        }
        ChangementObservé();
    }

    /// <returns><paramref name="c"/> représente un <see cref="Capteur"/></returns>
    public bool EstCapteur(char c)
    {
        lock (ObservateurMouvements) // Un Capteur est un observateur de mouvements
            return ObservateurMouvements.AnyValid(om => om is Capteur cap && cap.Symbole == c);
    }
    #endregion Capteurs

    #region IObservateursMouvements

    private readonly List<IObservateurMouvement> ObservateurMouvements = new();

    /// <summary>
    /// Service par lequel un observateur de mouvement s'abonne à la Carte
    /// </summary>
    /// <param name="mouvement"></param>
    public void Abonner(IObservateurMouvement mouvement)
    {
        lock (ObservateurMouvements) // Lock if someone is iterating through the list
            ObservateurMouvements.Add(mouvement);
    }

    /// <summary>
    /// Signale qu'un mouvement a été réalisé
    /// </summary>
    private void ChangementObservé()
    {
        lock (ObservateurMouvements) // Lock if someone adds an item while this is running
            ObservateurMouvements.ForEach((item) => item.MouvementObservé(this));
    }

    #endregion ObservateursMouvements

    #region Mouvement
    /// <summary>
    /// Détermine si le mouvement dirigé par <paramref name="choix"/> est valide
    /// </summary>
    internal bool IsMovementValid(Protagoniste protagoniste, Choix choix, out Point newPosition)
    {
        if (choix == Choix.Rien || choix == Choix.Quitter) // Aucun mouvement donc toujours bon
        {
            newPosition = protagoniste.Position;
            return true;
        }

        newPosition = RèglesDéplacement.ProchainePosition(protagoniste.Position, choix);

        return EstDisponible(newPosition);
    }

    #endregion Mouvement

    #region Character Finding / Identification

    /// <returns>Toutes les positions sur la Carte ayant <paramref name="symbole"/> comme valeur</returns>
    public List<Point> Trouver(char symbole) => FindMatchings(c => c.Equals(symbole));

    /// <returns>Toutes les positions sur la Carte respectant <paramref name="condition"/></returns>
    private List<Point> FindMatchings(Predicate<char> condition)
    {
        var validPositions = new List<Point>();

        ForEachTile((x, y) =>
        {
            var pos = new Point(x, y);
            if (Respecte(pos, condition))
                validPositions.Add(pos);
        });

        return validPositions;
    }

    /// <summary>
    /// Parcours toutes les cases de la carte de manière synchronisée
    /// </summary>
    /// <param name="actionX">Action exécutée à chaque case</param>
    /// <param name="actionY">Action exécutée à chaque ligne</param>
    /// Permet de centraliser la recherche dans la carte
    private void ForEachTile(Action<int, int>? actionX = null, Action<int>? actionY = null)
    {
        lock (this)
        {
            for (int y = 0; y < Hauteur; ++y)
            {
                for (int x = 0; x < Largeur; ++x)
                {
                    actionX?.Invoke(x, y);
                }

                actionY?.Invoke(y);
            }
        }
    }

    /// <summary>
    /// Applique tous les <paramref name="tests"/> à la position <paramref name="pos"/>
    /// </summary>
    /// <returns>(Success; Value)</returns>
    //public (bool, char) AppliquerTests(Point pos, params Func<char, bool>[] tests)
    //{
    //    char value = this[pos];
    //    bool isFailling = false;

    //    foreach (var item in tests)
    //    {
    //        if (item.Invoke(value))
    //        {
    //            isFailling = true;
    //            break;
    //        }
    //    }

    //    return isFailling ? (false, default) : (true, value);
    //}

    private bool Respecte(Point pos, Predicate<char> pred) => pred(this[pos]); // Merci pour la DLL
    private bool Respecte(Point pos, char target) => Respecte(pos, (char c) => c.Equals(target));

    #endregion Character Finding / Identifiacation

    #region Protagoniste
    /// <summary>
    /// Positionne <paramref name="obst"/> à sa position si cette dernière est disponible
    /// </summary>
    public void Installer(Protagoniste obst)
    {
        var pos = obst.Position;

        // Si la position est hors de la map, pas besoin d'attendre pour le lock
        if (!EstDans(pos))
            return;

        lock (this)
        {
            // Comme EstDisponible fait son propre lock sur this,
            // je dois faire la vérification manuellement
            if (Respecte(pos, Symboles.VIDE))
                this[pos] = obst.Symbole;
        }
    }

    /// <summary>
    /// Service appliquant le choix de mouvement de <paramref name="protagoniste"/>
    /// </summary>
    public void Appliquer(Protagoniste protagoniste, Choix choix)
    {
        // Skip if no mouvement
        if (choix == Choix.Rien || choix == Choix.Quitter)
            return;

        var newPosition = RèglesDéplacement.ProchainePosition(protagoniste.Position, choix);

        if (newPosition.Distance(protagoniste.Position) != RèglesDéplacement.MOVEMENT_DISTANCE)
            return; // Mouvement invalide

        if (!EstDans(newPosition))
            return; // Mouvement invalide

        lock (this)
        {
            if (!Respecte(newPosition, Symboles.VIDE)) // Mouvement vers une case non vide
            {
                protagoniste.Collision();
                return;
            }

            // Vider la case où se tenait le protagoniste
            this[protagoniste.Position] = Symboles.VIDE;

            // Afficher le bon character à la position du Protagoniste
            this[newPosition] = protagoniste.ObtenirSymbole(this);

            // Déplacer le protagoniste
            protagoniste.Déplacer(newPosition);
        }

        // Signalé le mouvement
        ChangementObservé();
    }
    #endregion

    /// <returns>Copie du contenu de la carte</returns>
    public char[,] Snapshot()
    {
        char[,] copyMap = new char[Hauteur, Largeur];

        ForEachTile((x, y) => copyMap[y, x] = this[x, y]);

        return copyMap;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string text = "";

        ForEachTile((x, y) => text += this[x, y], (_) => text += "\n");

        return text;
    }

    #region Exceptions
    public class PositionIllégaleException : Exception  { }
    public class PasDeCarteException : Exception { }
    public class CarteNonRectangulaireException : Exception { }
    public class SymboleManquantException : Exception { }
    public class SymboleIllégalException : Exception { }
    #endregion
}