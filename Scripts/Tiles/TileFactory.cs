using System;
using Godot;

namespace FarmGame.Tiles
{
    public enum TileType
    {
        GRASS,
        STONE,
        DIRT
    }

    public class TileFactory()
    {
        public static Tile InstantiateTile(TileType type)
        {
            PackedScene scene = type switch
            {
                TileType.GRASS => GrassTile.GetScene(),
                TileType.STONE => StoneTile.GetScene(),
                TileType.DIRT => DirtTile.GetScene(),
                _ => throw new Exception($"Unknown tile type '{type}'"),
            };
            Tile tile = scene.Instantiate<Tile>();
            return tile;
        }

        public static Tile GetRandomTile()
        {
            Array tileTypes = Enum.GetValues(typeof(TileType));
            Random random = new();
            TileType type = (TileType)tileTypes.GetValue(random.Next(tileTypes.Length));
            return InstantiateTile(type);
        }
    }
}