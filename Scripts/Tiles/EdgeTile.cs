using FarmGame.Scripts.Environment;
using Godot;

namespace FarmGame.Scripts.Tiles
{
    public partial class EdgeTile(Field field = null) : Tile(field)
    {
        public override TileType TileType => TileType.Edge;
        public override float Height => 1.0f;
    }
}
