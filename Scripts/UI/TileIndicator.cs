using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.UI
{

    public partial class TileIndicator : Node3D
    {
        [Export(PropertyHint.Range, "0, 0.1, 0.01")]
        public float LerpStrength = 0.01f;
        private Vector3 targetPosition;

        public override void _Ready()
        {
            targetPosition = new Vector3(0,0,0);   
        }

        public override void _Process(double delta)
        {
            GlobalPosition = GlobalPosition.Lerp(targetPosition, LerpStrength);
        }

        public void SetTargetTile(Tile tile)
        {
            targetPosition = new(
                tile.GridPosition.X + 0.5f,
                tile.GetHeight() + 0.1f,
                tile.GridPosition.Y + 0.5f
            );
        }
    }
}
