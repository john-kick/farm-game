using System.Collections.Generic;
using FarmGame.Scripts.Tiles;
using FarmGame.Scripts.Utils;

namespace FarmGame.Scripts.Field.Chunks;

/// <summary>
/// A chunk represents a a section of a field. Each chunk contains 16x16 Tiles.
/// </summary>
public class Chunk(Vector2I chunkPosition)
{
    private Vector2I _chunkPosition = chunkPosition;
    private readonly Dictionary<Vector2I, Tile> tiles = [];

    /// <summary>
    /// Return the tile at the given position.
    /// </summary>
    /// <exception cref="MissingTileException"></exception>
    /// <exception cref="OutOfChunkBoundsException"></exception>
    public Tile GetTile(Vector2I position)
    {
        AssertIsInChunk(position);
        if (!tiles.TryGetValue(position, out Tile tile))
            throw new MissingTileException($"Tile missing at {position}.");
        return tile;
    }

    /// <summary>
    /// Set the tile at the given position
    /// </summary>
    /// <exception cref="OutOfChunkBoundsException"></exception>
    public void SetTile(Vector2I position, Tile tile)
    {
        AssertIsInChunk(position);
        tiles[position] = tile;
    }

    /// <summary>
    /// Asserts if the given position is within the chunk bounds
    /// </summary>
    /// <exception cref="OutOfChunkBoundsException"></exception>
    public static void AssertIsInChunk(Vector2I position)
    {
        if (!IsInChunk(position))
            throw new OutOfChunkBoundsException($"Cannot access position {position}");
    }

    /// <summary>
    /// Checks if the given position is within the chunk bounds
    /// </summary>
    public static bool IsInChunk(Vector2I position)
    {
        return 0 <= position.X && position.X < 16
            && 0 <= position.Y && position.Y < 16;
    }
}