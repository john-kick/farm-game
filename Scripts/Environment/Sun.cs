using Godot;

namespace Scripts.Environment
{
	public partial class Sun : Node3D
	{
		[Export] public float RotationSpeed = 0.2f;
		private DirectionalLight3D sun;

		public override void _Ready()
		{
			sun = GetNode<DirectionalLight3D>("Sun");
		}

		public override void _Process(double delta)
		{
			// rotate pivot around Y
			GlobalRotate(Vector3.Up, (float)(RotationSpeed * delta));
		}
	}
}
