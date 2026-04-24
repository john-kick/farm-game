using System.Collections.Generic;
using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.Field
{
    public class Field
    {
        [Export] public Vector2I Size;

        private readonly Dictionary<Vector2I, Tile> tiles;

        public Tile GetTile(Vector2I position)
        {
            AssertIsInField(position);
            return tiles.GetValueOrDefault(position, null);
        }

        public void SetTile(Vector2I position, Tile tile)
        {
            AssertIsInField(position);
            tiles.Add(position, tile);
        }

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