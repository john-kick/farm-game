using FarmGame.Scripts.Utils;

namespace FarmGame.Scripts.Field;

/// <summary>
/// Encodes neighbor directions as bit flags.
/// Bit positions:
/// - 0: North (0, -1)
/// - 1: North-East (1, -1)
/// - 2: East (1, 0)
/// - 3: South-East (1, 1)
/// - 4: South (0, 1)
/// - 5: South-West (-1, 1)
/// - 6: West (-1, 0)
/// - 7: North-West (-1, -1)
/// </summary>
public struct NeighborMask
{
    public const byte North = 1 << 0;     // 0x01
    public const byte NorthEast = 1 << 1; // 0x02
    public const byte East = 1 << 2;      // 0x04
    public const byte SouthEast = 1 << 3; // 0x08
    public const byte South = 1 << 4;     // 0x10
    public const byte SouthWest = 1 << 5; // 0x20
    public const byte West = 1 << 6;      // 0x40
    public const byte NorthWest = 1 << 7; // 0x80

    // N, NE, E, SE, S, SW, W, NW
    public static readonly Vector2I[] directions = [
        new(0, -1),  // North
        new(1, -1),  // North-East
        new(1, 0),   // East
        new(1, 1),   // South-East
        new(0, 1),   // South
        new(-1, 1),  // South-West
        new(-1, 0),  // West
        new(-1, -1)  // North-West
    ];

    public static readonly byte[] bitFlags = [
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    ];
}
