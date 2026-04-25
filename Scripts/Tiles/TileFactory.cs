using System;

namespace FarmGame.Scripts.Tiles
{
	public static class TileFactory
	{
		private static readonly Random _random = new();

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
				_ => throw new Exception($"Unsupported tile type: {type}")
			};
		}

		public static Tile CreateRandomTile()
		{
			var values = Enum.GetValues(typeof(TileType));
			var randomType = (TileType)values.GetValue(_random.Next(values.Length))!;
			return CreateTile(randomType);
		}
	}
}
