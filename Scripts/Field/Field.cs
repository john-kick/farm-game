using System.Collections.Generic;
using FarmGame.Scripts.Tiles;
using FarmGame.Scripts.Utils;

namespace FarmGame.Scripts.Field
{
    /// <summary>
    /// A collection of Tiles
    /// </summary>
    /// <param name="size">The amount of tiles in the X and Z direction</param>
    public class Field(Vector2I size)
    {
        private Vector2I Size = size;
        private readonly Dictionary<Vector2I, Tile> tiles = [];

        /// <summary>
        /// Returns the tile located at the given position
        /// </summary>
        /// <exception cref="MissingTileException"></exception>
        /// <exception cref="OutOfFieldBoundsException"></exception>
        public Tile GetTile(Vector2I position)
        {
            AssertIsInField(position);
            if (!tiles.TryGetValue(position, out Tile tile))
                throw new MissingTileException($"Tile missing at {position}");
            return tile;
        }

        /// <summary>
        /// Sets the given tile in the field's tile dictionary
        /// </summary>
        /// <exception cref="OutOfFieldBoundsException"></exception>
        public void SetTile(Vector2I position, Tile tile)
        {
            AssertIsInField(position);
            tiles[position] = tile;
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
        /// Asserts if the given position is within the field's bounds
        /// </summary>
        /// <exception cref="OutOfFieldBoundsException"></exception>
        public void AssertIsInField(Vector2I position)
        {
            if (!IsInField(position))
                throw new OutOfFieldBoundsException($"Cannot access position {position}.");
        }

        /// <summary>
        /// Checks if the given position is inside the field
        /// </summary>
        public bool IsInField(Vector2I position)
        {
            return position.X >= 0 && position.X < Size.X
                && position.Y >= 0 && position.Y < Size.Y;
        }
    }
}