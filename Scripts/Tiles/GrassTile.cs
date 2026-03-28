using Godot;

namespace FarmGame.Scripts.Tiles
{
	public partial class GrassTile : Tile
	{
		[Export] public Color GrassColor = new Color(0.2f, 0.8f, 0.2f);

		public override TileType TileType => TileType.Grass;
		public override float Height => 0.95f;
	}
}
