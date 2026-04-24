using System;

#nullable enable

namespace FarmGame.Scripts.Utils;

/// <summary>
/// A 2D vector with integer coordinates.
/// Provides the same functionality as Godot's Vector2I but implemented independently.
/// </summary>
public struct Vector2I(int x, int y) : IEquatable<Vector2I>, IComparable<Vector2I>
{
    public int X = x;
    public int Y = y;

    /// <summary>
    /// Returns the length (magnitude) of this vector.
    /// </summary>
    public readonly float Length => MathF.Sqrt(X * X + Y * Y);

    /// <summary>
    /// Returns the squared length of this vector. Faster than Length for comparisons.
    /// </summary>
    public readonly int LengthSquared => X * X + Y * Y;

    /// <summary>
    /// Returns the absolute (component-wise) value of this vector.
    /// </summary>
    public readonly Vector2I Abs => new(Math.Abs(X), Math.Abs(Y));

    /// <summary>
    /// Returns the sign of each component: -1, 0, or 1.
    /// </summary>
    public readonly Vector2I Sign => new(Math.Sign(X), Math.Sign(Y));

    /// <summary>
    /// Returns the minimum of X and Y.
    /// </summary>
    public readonly int MinAxisValue => Math.Min(X, Y);

    /// <summary>
    /// Returns the index of the axis with the maximum absolute value (0 for X, 1 for Y).
    /// </summary>
    public readonly int MaxAxis => Math.Abs(X) >= Math.Abs(Y) ? 0 : 1;

    /// <summary>
    /// Returns the index of the axis with the minimum absolute value (0 for X, 1 for Y).
    /// </summary>
    public readonly int MinAxis => Math.Abs(X) < Math.Abs(Y) ? 0 : 1;

    /// <summary>
    /// Returns the component at the given axis (0 for X, 1 for Y).
    /// </summary>
    public int this[int axis]
    {
        readonly get => axis switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException(nameof(axis), "Axis must be 0 or 1")
        };
        set
        {
            switch (axis)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), "Axis must be 0 or 1");
            }
        }
    }

    /// <summary>
    /// Returns the dot product of this vector and another.
    /// </summary>
    public readonly int Dot(Vector2I other) => X * other.X + Y * other.Y;

    /// <summary>
    /// Returns a vector with each component limited to the given range.
    /// </summary>
    public readonly Vector2I Clamp(Vector2I min, Vector2I max) =>
        new(Math.Clamp(X, min.X, max.X), Math.Clamp(Y, min.Y, max.Y));

    /// <summary>
    /// Returns the distance to another vector.
    /// </summary>
    public readonly float DistanceTo(Vector2I other)
    {
        int dx = X - other.X;
        int dy = Y - other.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Returns the squared distance to another vector. Faster than DistanceTo for comparisons.
    /// </summary>
    public readonly int DistanceSquaredTo(Vector2I other)
    {
        int dx = X - other.X;
        int dy = Y - other.Y;
        return dx * dx + dy * dy;
    }

    /// <summary>
    /// Returns a vector linearly interpolated between this vector and another.
    /// </summary>
    public readonly Vector2I Lerp(Vector2I to, float weight) =>
        new(
            (int)Math.Round(X + (to.X - X) * weight),
            (int)Math.Round(Y + (to.Y - Y) * weight)
        );

    /// <summary>
    /// Returns a vector with the maximum component values from this and another vector.
    /// </summary>
    public readonly Vector2I Max(Vector2I other) =>
        new(Math.Max(X, other.X), Math.Max(Y, other.Y));

    /// <summary>
    /// Returns a vector with the minimum component values from this and another vector.
    /// </summary>
    public readonly Vector2I Min(Vector2I other) =>
        new(Math.Min(X, other.X), Math.Min(Y, other.Y));

    public override readonly string ToString() => $"({X}, {Y})";

    public override readonly bool Equals(object? obj) => obj is Vector2I other && Equals(other);

    public readonly bool Equals(Vector2I other) => X == other.X && Y == other.Y;

    public override readonly int GetHashCode() => HashCode.Combine(X, Y);

    public readonly int CompareTo(Vector2I other)
    {
        int xComparison = X.CompareTo(other.X);
        return xComparison != 0 ? xComparison : Y.CompareTo(other.Y);
    }

    // Operators
    public static Vector2I operator +(Vector2I a, Vector2I b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2I operator -(Vector2I a, Vector2I b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2I operator *(Vector2I a, Vector2I b) => new(a.X * b.X, a.Y * b.Y);
    public static Vector2I operator *(Vector2I a, int scalar) => new(a.X * scalar, a.Y * scalar);
    public static Vector2I operator *(int scalar, Vector2I a) => new(scalar * a.X, scalar * a.Y);
    public static Vector2I operator /(Vector2I a, Vector2I b) => new(a.X / b.X, a.Y / b.Y);
    public static Vector2I operator /(Vector2I a, int scalar) => new(a.X / scalar, a.Y / scalar);
    public static Vector2I operator %(Vector2I a, Vector2I b) => new(a.X % b.X, a.Y % b.Y);
    public static Vector2I operator %(Vector2I a, int scalar) => new(a.X % scalar, a.Y % scalar);
    public static Vector2I operator --(Vector2I a) => new(a.X - 1, a.Y - 1);
    public static Vector2I operator ++(Vector2I a) => new(a.X + 1, a.Y + 1);
    public static Vector2I operator -(Vector2I a) => new(-a.X, -a.Y);

    public static bool operator ==(Vector2I a, Vector2I b) => a.Equals(b);
    public static bool operator !=(Vector2I a, Vector2I b) => !a.Equals(b);
    public static bool operator <(Vector2I a, Vector2I b) => a.CompareTo(b) < 0;
    public static bool operator <=(Vector2I a, Vector2I b) => a.CompareTo(b) <= 0;
    public static bool operator >(Vector2I a, Vector2I b) => a.CompareTo(b) > 0;
    public static bool operator >=(Vector2I a, Vector2I b) => a.CompareTo(b) >= 0;

    // Zero and One constants
    public static readonly Vector2I Zero = new(0, 0);
    public static readonly Vector2I One = new(1, 1);
    public static readonly Vector2I Left = new(-1, 0);
    public static readonly Vector2I Right = new(1, 0);
    public static readonly Vector2I Up = new(0, -1);
    public static readonly Vector2I Down = new(0, 1);
}
