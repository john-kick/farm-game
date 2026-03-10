using System;
using System.Linq;
using Godot;

namespace FarmGame.Tiles
{
	public enum TileType
	{
		GRASS,
		STONE,
		DIRT,
		BASE
	}

	public class TileFactory()
	{
		public static Tile GetTile(TileType type)
		{
			PackedScene scene = type switch
			{
				TileType.GRASS => GrassTile.GetScene(),
				TileType.STONE => StoneTile.GetScene(),
				TileType.DIRT => DirtTile.GetScene(),
				TileType.BASE => BaseTile.GetScene(),
				_ => throw new Exception($"Unknown tile type '{type}'"),
			};
			Tile tile = scene.Instantiate<Tile>();
			return tile;
		}

		public static Tile GetRandomTile()
		{
			Random random = new();
			TileType[] tileTypes = [.. Enum.GetValues<TileType>().Where(t => t != TileType.BASE)];

			TileType type = tileTypes[random.Next(tileTypes.Length)];
			return GetTile(type);
		}
	}
}
