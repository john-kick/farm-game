using FarmGame.Scripts.Environment;

namespace FarmGame.Scripts.Tiles
{
	public static class TileFactory
	{
		/// <summary>
		/// Create a tile of the specified type
		/// </summary>
		public static Tile CreateTile(TileType type)
		{
			return type switch
			{
				TileType.Grass => new GrassTile(),
				TileType.Dirt => new DirtTile(),
				TileType.Stone => new StoneTile(),
				TileType.Edge => new EdgeTile(),
				_ => throw new System.Exception($"Unsupported tile type: {type}")
			};
		}
	}
}
