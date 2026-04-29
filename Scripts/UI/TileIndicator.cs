using FarmGame.Scripts.Environment;
using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.UI
{
    public partial class TileIndicator : MeshInstance3D
    {
        [Export] public float LerpWeight = 0.02f;

        private Vector3 targetPosition;


        public override void _Ready()
        {
            Visible = false;
        }

        public override void _Process(double delta)
        {
            Position = GlobalPosition.Lerp(targetPosition, LerpWeight);
        }

        public void SetTargetPosition(Vector3 position)
        {
            targetPosition = position;
        }

        public void TargetTile(Tile tile)
        {
            float tileTop = tile != null ? tile.Height : 0f;
            targetPosition = Field.GridToWorldPosition(tile.GridPosition) + new Vector3(0.5f, tileTop + 0.1f, 0.5f);
        }
    }
}
