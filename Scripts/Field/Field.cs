using System.Collections.Generic;
using FarmGame.Scripts.Field.Chunks;
using FarmGame.Scripts.Tiles;
using FarmGame.Scripts.Utils;

namespace FarmGame.Scripts.Field;

/// <summary>
/// A collection of Tiles
/// </summary>
/// <param name="size">The amount of chunks in the X and Z direction</param>
public class Field
{
    private readonly int CHUNK_SIZE = 16;
    private readonly ChunkRegistry chunkRegistry = new();

    /// <summary>
    /// Returns the tile located at the given position from the chunk
    /// </summary>
    /// <exception cref="MissingTileException"></exception>
    /// <exception cref="OutOfChunkBoundsException"></exception>
    public Tile GetTile(Vector2I position)
    {
        Chunk chunk = GetChunkFromTilePosition(position);
        Vector2I localPos = ToLocalChunkPosition(position);
        return chunk.GetTile(localPos);
    }

    /// <summary>
    /// Sets the given tile in the chunk for the given position
    /// </summary>
    /// <exception cref="OutOfChunkBoundsException"></exception>
    public void SetTile(Vector2I position, Tile tile)
    {
        Chunk chunk = GetChunkFromTilePosition(position);
        Vector2I localPos = ToLocalChunkPosition(position);
        chunk.SetTile(localPos, tile);
    }

    /// <summary>
    /// Returns the surrounding (up to 8) neighbors of the tile at the given position
    /// </summary>
    public IEnumerable<Tile> GetNeighbors(Vector2I position)
    {
        List<Tile> neighbors = [];

        for (int z = -1; z <= 1; z++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && z == 0)
                    // Don't include self
                    continue;

                Vector2I offset = new(x, z);
                try
                {
                    neighbors.Add(GetTile(position + offset));
                }
                catch (OutOfFieldBoundsException)
                {
                    // Don't include oob position
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Returns the chunk which contains the tile at tilePos
    /// </summary>
    private Chunk GetChunkFromTilePosition(Vector2I tilePos)
    {
        Vector2I chunkCoord = GetChunkCoordinate(tilePos);
        return chunkRegistry.GetChunk(chunkCoord);
    }

    private Vector2I GetChunkCoordinate(Vector2I tilePos) => new(tilePos.X / CHUNK_SIZE, tilePos.Y / CHUNK_SIZE);
    private Vector2I ToLocalChunkPosition(Vector2I tilePos) => new(tilePos.X % CHUNK_SIZE, tilePos.X % CHUNK_SIZE);
}