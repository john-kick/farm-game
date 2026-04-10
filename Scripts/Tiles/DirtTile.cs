using FarmGame.Scripts.Controls.Interactions;
using FarmGame.Scripts.Environment;
using Godot;

namespace FarmGame.Scripts.Tiles
{
	public partial class DirtTile(Field field = null) : Tile(field)
	{
		[Export] public Color DirtColor = new(0.6f, 0.4f, 0.2f);

		public override TileType TileType => TileType.Dirt;
		public override float Height => 0.85f;
		public override Material Material => new StandardMaterial3D() { AlbedoColor = DirtColor };

        public override ReplaceTileInteraction PrimaryInteraction()
        {
            return CreateReplaceTileInteraction(TileType.Grass);
        }
	}
}
