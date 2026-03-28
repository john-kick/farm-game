using Godot;

namespace FarmGame.Scripts.Tiles
{
	public partial class DirtTile : Tile
	{
		[Export] public Color DirtColor = new Color(0.6f, 0.4f, 0.2f);

		public override TileType TileType => TileType.Dirt;
		public override float Height => 0.85f;
	}
}
