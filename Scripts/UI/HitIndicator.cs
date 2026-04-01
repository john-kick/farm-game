using Godot;

namespace FarmGame.Scripts.UI
{
    public partial class HitIndicator : MeshInstance3D
    {
        [Export] public float LerpWeight = 0.1f;

        public override void _Ready()
        {
            Visible = false;
            Mesh = new BoxMesh()
            {
                Size = Vector3.One * 0.1f
            };
            MaterialOverride = new StandardMaterial3D()
            {
                AlbedoColor = Colors.Red
            };
        }
    }
}