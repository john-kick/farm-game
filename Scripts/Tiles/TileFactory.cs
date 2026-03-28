using Godot;

namespace FarmGame.Scripts.Tiles
{
	public static class TileFactory
	{
		/// <summary>
		/// Create a tile of the specified type
		/// </summary>
		public static Tile CreateTile(TileType type, Vector2I gridPos)
		{
			Tile tile = type switch
			{
				TileType.Grass => new GrassTile(),
				TileType.Dirt => new DirtTile(),
				TileType.Stone => new StoneTile(),
				_ => new GrassTile()
			};

			tile.GridPosition = gridPos;
			return tile;
		}
	}
}
