using Godot;

namespace FarmGame.Scripts.Fences
{
	public partial class Fence : Node3D
	{
		public void Initialize(Vector3 _Position)
		{
			Position = _Position + Vector3.Up * 1.5f;
		}

		public static Fence GetSceneInstance()
		{
			return GD.Load<PackedScene>("res://Scenes/fence/fencepost.tscn").Instantiate<Fence>();
		}

		public void ConnectToFence(Fence other)
		{
			if (other == null)
				return;

			// Load the fence board
			Node3D board = GD.Load<PackedScene>("res://Scenes/fence/fenceboard.tscn").Instantiate<Node3D>();
			
			// Get the direction to the other fence
			Vector3 direction = (other.Position - Position).Normalized();

			// Place the board halfway to the other fence along the direction.
			board.Position = direction * (Position.DistanceTo(other.Position) / 2);

			AddChild(board);

			// Rotate the board to face the other fence, then rotate it 90 degrees to be perpendicular to the direction.
			board.LookAt(other.Position, Vector3.Up);
			board.Rotate(Vector3.Up, Mathf.Pi / 2);
		}
	}
}
