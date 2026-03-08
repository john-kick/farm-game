using Godot;

namespace FarmGame.Tiles
{
    public partial class DirtTile : Tile
    {
        public override Material GetMaterial()
        {
            return new StandardMaterial3D()
            {
                AlbedoColor = Colors.SandyBrown
            };
        }

        public static new PackedScene GetScene()
        {
            return GD.Load<PackedScene>("res://scenes/tiles/dirt_tile.tscn");
        }

        protected override float GetHeight()
        {
            return 0.7f;
        }

        public override TileType GetTileType()
        {
            return TileType.DIRT;
        }
    }
}