using System;
using System.Collections.Generic;
using FarmGame.Scripts.Utils;

namespace FarmGame.Scripts.Field.Chunks;

/// <summary>
/// Manages multiple chunks and provides an API to create and manage chunks
/// </summary>
public class ChunkRegistry
{
    private Dictionary<Vector2I, Chunk> chunks = [];

    /// <summary>
    /// Returns the chunk at the given position
    /// </summary>
    /// <exception cref="MissingChunkException"></exception>
    public Chunk GetChunk(Vector2I position)
    {
        if (!chunks.TryGetValue(position, out Chunk chunk))
            throw new MissingChunkException();
        return chunk;
    }

    /// <summary>
    /// Adds a new chunk at the given position and returns it
    /// </summary>
    public Chunk CreateChunk(Vector2I position)
    {
        Chunk chunk = new();
        chunks[position] = chunk;
        return chunk;
    }

    /// <summary>
    /// Removes the chunk at the given position
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void UnloadChunk(Vector2I position)
    {
        if (!chunks.Remove(position))
            throw new Exception($"Could not unload chunk at {position}");
    }

    /// <summary>
    /// Removes all chunks
    /// </summary>
    public void Clear() => chunks = [];
}
