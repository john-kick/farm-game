using FarmGame.Scripts.Environment;

namespace FarmGame.Scripts.Tiles
{
	public static class TileFactory
	{
		/// <summary>
		/// Create a tile of the specified type
		/// </summary>
		public static Tile CreateTile(TileType type, Field field = null)
		{
			return type switch
			{
				TileType.Grass => new GrassTile(field),
				TileType.Dirt => new DirtTile(field),
				TileType.Stone => new StoneTile(field),
				TileType.Edge => new EdgeTile(field),
				_ => throw new System.Exception($"Unsupported tile type: {type}")
			};
		}
	}
}
