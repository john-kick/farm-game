using FarmGame.Scripts.Environment;
using Godot;

namespace FarmGame.Scripts.Tiles
{
    public partial class EdgeTile(Field field = null) : Tile(field)
    {
        [Export] public Color EdgeColor = new(0.13f, 0.52f, 0.13f);

        public override TileType TileType => TileType.Edge;
        public override float Height => 1.0f;
        public override Material Material => new StandardMaterial3D() { AlbedoColor = EdgeColor };
    }
}
