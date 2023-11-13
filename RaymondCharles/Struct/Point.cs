/// (DD/MM/YYYY) AUTHOR:
/// 13/11/2023 SAMUEL GAUTHIER:
/// - Allowed to compare two object if they are Point (Object.Equals())
/// 
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Made 'X' and 'Y' modifiable after initialisation
/// - Removed 'Point' read only attribute
/// 
/// 21/10/2023 SAMUEL GAUTHIER:
/// - Made Point and Distance() readonly

namespace RaymondCharles.Struct;

/// <summary>
/// Coordonnée 2D
/// </summary>
public struct Point : IEquatable<Point>
{
    public int X { get; set; }
    public int Y { get; set; }

    // Pas de constructeur primaire ;-;
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <inheritdoc/>
    public override readonly string ToString() => $"Point {{{X}; {Y}}}";

    /// <inheritdoc/>
    public readonly bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);

    /// <inheritdoc/>
    /// Dans le cas où 
    public readonly override bool Equals(object? obj) => obj is Point p ? Equals(p) : base.Equals(obj);

    /// <inheritdoc/>
    public readonly override int GetHashCode() => base.GetHashCode();

    /// <inheritdoc/>
    public static bool operator ==(Point c1, Point c2) => c1.Equals(c2);
    /// <inheritdoc/>
    public static bool operator !=(Point c1, Point c2) => !(c1 == c2);

    /// <inheritdoc/>
    public static Point operator +(Point c1, Point c2) => new(c1.X + c2.X, c1.Y + c2.Y);

    /// <inheritdoc/>
    public static Point operator -(Point c1, Point c2) => new(c1.X - c2.X, c1.Y - c2.Y);

    /// <inheritdoc/>
    public static Point operator -(Point c1) => new(-c1.X, -c1.Y);

    /// <inheritdoc/>
    public static Point operator /(Point c1, Point c2) => new(c1.X / c2.X, c1.Y / c2.Y);

    /// <inheritdoc/>
    public static Point operator *(Point c1, Point c2) => new(c1.X * c2.X, c1.Y * c2.Y);

    /// <returns>Distance entre ce point et <paramref name="destination"/></returns>
    public double Distance(Point destination) => Math.Sqrt(Math.Pow(X - destination.X, 2) + Math.Pow(Y - destination.Y, 2));

    /// <returns>Distance entre ce point et le point (<paramref name="x"/>; <paramref name="y"/>)</returns>
    public double Distance(int x, int y) => Distance(new Point(x, y));
}
