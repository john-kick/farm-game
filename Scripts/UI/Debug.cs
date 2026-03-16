using Godot;

namespace FarmGame.Scripts.UI
{
    public partial class Debug : CanvasLayer
    {
        [Export] public Node3D player;
        private Label label;

        public override void _Ready()
        {
            label = GetNode<Label>("PlayerPosition");
        }

        public override void _Process(double delta)
        {
            Vector3 pos = player.GlobalTransform.Origin;
            label.Text = $"Player Position: X: {pos.X}  Y: {pos.Y}  Z: {pos.Z}";
        }
    }
}