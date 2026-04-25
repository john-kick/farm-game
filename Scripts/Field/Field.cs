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
    public static readonly int CHUNK_SIZE = 16;
    private readonly ChunkRegistry chunkRegistry = new();

    public void GenerateField(int chunksX, int chunksZ, TileType? type)
    {
        chunkRegistry.Clear();

        for (int z = 0; z < chunksZ; z++)
        {
            for (int x = 0; x < chunksX; x++)
            {
                Vector2I position = new(x, z);
                Chunk chunk = chunkRegistry.CreateChunk(position);
                chunk.Fill(type);
            }
        }
    }

    /// <summary>
    /// Returns the tile located at the given position from the chunk
    /// </summary>
    /// <exception cref="MissingTileException"></exception>
    /// <exception cref="OutOfChunkBoundsException"></exception>
    public Tile GetTile(Vector2I position)
    {
        Chunk chunk = GetChunkFromTilePosition(position);
        Vector2I localPos = LocalTilePositionFromGlobalTilePosition(position);
        return chunk.GetTile(localPos);
    }

    /// <summary>
    /// Sets the given tile in the chunk for the given position
    /// </summary>
    /// <exception cref="OutOfChunkBoundsException"></exception>
    public void SetTile(Vector2I position, Tile tile)
    {
        Chunk chunk = GetChunkFromTilePosition(position);
        Vector2I localPos = LocalTilePositionFromGlobalTilePosition(position);
        chunk.SetTile(localPos, tile);
    }

    /// <summary>
    /// Returns a bitmask of which neighboring tiles exist, excluding out-of-bounds tiles.
    /// Each bit represents a direction: bit 0 = North, bit 1 = NE, ..., bit 7 = NW
    /// </summary>
    public byte GetNeighborsMask(Vector2I position)
    {
        byte mask = 0;

        for (int i = 0; i < NeighborMask.directions.Length; i++)
        {
            try
            {
                GetTile(position + NeighborMask.directions[i]);
                mask |= NeighborMask.bitFlags[i];
            }
            catch (OutOfChunkBoundsException)
            {
                // Neighbor doesn't exist, skip
            }
        }

        return mask;
    }

    /// <summary>
    /// Returns the surrounding (up to 8) neighbors of the tile at the given position
    /// </summary>
    /// <exception cref="MissingTileException"></exception>
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
                catch (OutOfChunkBoundsException)
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
        Vector2I chunkCoord = ChunkPositionFromGlobalTilePosition(tilePos);
        return chunkRegistry.GetChunk(chunkCoord);
    }

    /// <summary>
    /// Calculates the chunk position from the given global tile position
    /// </summary>
    private static Vector2I ChunkPositionFromGlobalTilePosition(Vector2I globalTilePosition)
        => new(globalTilePosition.X / CHUNK_SIZE, globalTilePosition.Y / CHUNK_SIZE);

    /// <summary>
    /// Calculates the tile position within a chunk from the given global tile position
    /// </summary>
    private static Vector2I LocalTilePositionFromGlobalTilePosition(Vector2I globalTilePosition)
        => new(globalTilePosition.X % CHUNK_SIZE, globalTilePosition.Y % CHUNK_SIZE);

    /// <summary>
    /// Calculates the global tile position from the given chunk position and local tile position within that chunk
    /// </summary>
    private static Vector2I GlobalTilePositionFromChunkAndLocal(
        Vector2I chunkPosition,
        Vector2I localTilePosition)
    => new(
        chunkPosition.X * CHUNK_SIZE + localTilePosition.X,
        chunkPosition.Y * CHUNK_SIZE + localTilePosition.Y
    );
}