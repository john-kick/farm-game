using Godot;

namespace FarmGame.Tiles
{
    public partial class GrassTile : Tile
    {
        public override Material GetMaterial()
        {
            return new StandardMaterial3D()
            {
                AlbedoColor = Colors.Green
            };
        }

        public static new PackedScene GetScene()
        {
            return GD.Load<PackedScene>("res://Scenes/Tiles/grass_tile.tscn");
        }

        public override TileType GetTileType()
        {
            return TileType.GRASS;
        }
    }
}