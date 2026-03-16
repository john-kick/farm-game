using Godot;

namespace FarmGame.Scripts.Tiles
{
	public partial class StoneTile : Tile
	{
		public override Material GetMaterial()
		{
			return new StandardMaterial3D()
			{
				AlbedoColor = Colors.SlateGray
			};
		}

		public static new PackedScene GetScene()
		{
			return GD.Load<PackedScene>("res://Scenes/Tiles/stone_tile.tscn");
		}

		public override TileType GetTileType()
		{
			return TileType.STONE;
		}
	}
}
