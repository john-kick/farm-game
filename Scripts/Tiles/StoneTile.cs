using Godot;

namespace FarmGame.Scripts.Tiles
{
	public partial class StoneTile : Tile
	{
		[Export] public Color StoneColor = new Color(0.5f, 0.5f, 0.5f);

		public override TileType TileType => TileType.Stone;
		public override float Height => 0.9f;
	}
}
