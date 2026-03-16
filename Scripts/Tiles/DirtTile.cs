using Godot;

namespace FarmGame.Scripts.Tiles
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
            return GD.Load<PackedScene>("res://Scenes/Tiles/dirt_tile.tscn");
        }

        protected override float GetHeight()
        {
            return 0.9f;
        }

        public override TileType GetTileType()
        {
            return TileType.DIRT;
        }
    }
}