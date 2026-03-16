using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.UI
{

    public partial class TileIndicator : Node3D
    {
        private Vector3 targetPosition;

        public override void _Ready()
        {
            targetPosition = new Vector3(0,0,0);   
        }

        public override void _Process(double delta)
        {
            GlobalPosition = GlobalPosition.Lerp(targetPosition, 0.3f);
        }

        public void Show(Tile tile)
        {
            targetPosition = new(
                tile.GridPosition.X + 0.5f,
                tile.GetHeight() + 0.1f,
                tile.GridPosition.Y + 0.5f
            );
            Visible = true;
        }

        public void FHide()
        {
            Visible = false;
        }
    }
}
