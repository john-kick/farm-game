using Godot;

namespace FarmGame.Scripts.Tiles
{
    public partial class BaseTile : Tile
    {
        public override Material GetMaterial()
        {
            return new StandardMaterial3D()
            {
                AlbedoColor = Colors.Black
            };
        }

        public static new PackedScene GetScene()
        {
            return GD.Load<PackedScene>("res://Scenes/Tiles/base_tile.tscn");
        }

        protected override float GetHeight()
        {
            return 0;
        }

        public override TileType GetTileType()
        {
            return TileType.BASE;
        }
    }
}
