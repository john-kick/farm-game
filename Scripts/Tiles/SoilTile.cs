using FarmGame.Scripts.Controls.Interactions;
using FarmGame.Scripts.Environment;
using Godot;

namespace FarmGame.Scripts.Tiles
{
	public partial class SoilTile(Field field = null) : Tile(field)
	{
		public override TileType TileType => TileType.Soil;
		public override float Height => 0f;

		public override ReplaceTileInteraction PrimaryInteraction()
		{
			return CreateReplaceTileInteraction(TileType.Grass);
		}
	}
}
