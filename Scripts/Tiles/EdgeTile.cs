using Godot;

namespace FarmGame.Scripts.Tiles
{
    public partial class EdgeTile : Tile
    {
        public override Material GetMaterial()
        {
            return new StandardMaterial3D()
            {
                AlbedoColor = Colors.Black
            };
        }

        public override TileType GetTileType()
        {
            return TileType.EDGE;
        }

        public override float GetHeight()
        {
            return 1.1f;
        }

        public static new PackedScene GetScene()
        {
            return GD.Load<PackedScene>("res://Scenes/Tiles/edge_tile.tscn");
        }

    }
}