using RaymondCharles.Interfaces;
using RaymondCharles.Struct;

namespace RaymondCharles.Entities.Protagonistes;

public class Obstacle : Protagoniste
{
    private char symbole = 'B';

    public Obstacle(Point initialPosition, IAffichable? menu, char symbole) : base(initialPosition, menu)
    {
        this.symbole = symbole;
    }

    public Obstacle(Point initialPosition, IAffichable? menu, IDirecteur directeur, char symbole) : base(initialPosition, menu, directeur)
    {
        this.symbole = symbole;
    }

    public override char Symbole => symbole;
}