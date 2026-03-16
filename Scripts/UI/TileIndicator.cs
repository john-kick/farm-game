using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.UI
{
    public partial class TileIndicator : Node3D
    {
        public override void _Ready() {}

        public void Show(Tile tile)
        {
            Visible = true;
            Vector3 pos = new(
                tile.GridPosition.X + 0.5f,
                tile.GetHeight() + 0.1f,
                tile.GridPosition.Y + 0.5f
            );
            GlobalTransform = new Transform3D(Basis.Identity, pos);
            Visible = true;
        }

        public void FHide()
        {
            Visible = false;
        }
    }
}
