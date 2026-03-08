using Godot;

namespace FarmGame.Scripts.Player
{
	public partial class Player : CharacterBody3D
	{
		[Export] public float Speed = 1.0f;
		[Export] public float MouseSensitivity = .15f;
		[Export] public float MovementSpeed = 1.0f;
		[Export] public float Acceleration = .2f;
		[Export] public float Damping = .1f;
		[Export] public float StopThreshold = .0005f;
		[Export] public float FastMultiplier = 2f;
		[Export] public float Gravity = .981f;

		private Vector3 velocity;
		private Camera3D camera;

        public override void _Ready()
        {
			Input.MouseMode = Input.MouseModeEnum.Captured;
			camera = GetNode<Camera3D>("Camera");
        }

		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("ui_cancel"))
			{
				Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
					? Input.MouseModeEnum.Visible
					: Input.MouseModeEnum.Captured;
			}

			if (Input.MouseMode != Input.MouseModeEnum.Captured)
				return;

			if (@event is InputEventMouseMotion mouseMotion)
			{
				Vector3 targetRotation = new Vector3(mouseMotion.Relative.Y, mouseMotion.Relative.X, 0) * MouseSensitivity;
				RotationDegrees -= targetRotation;
				RotationDegrees = new Vector3(Mathf.Clamp(RotationDegrees.X, -89, 89), RotationDegrees.Y, RotationDegrees.Z);
			}
		}

		public override void _Process(double delta)
		{
			ApplyMovementInput(delta);
			// ApplyGravity(delta);
			UpdatePosition();
		}

		private void ApplyMovementInput(double delta)
		{
			float maxSpeed = MovementSpeed;

			if (Input.IsActionPressed("ui_shift"))
				maxSpeed *= FastMultiplier;

			Vector3 direction = Vector3.Zero;

			if (Input.IsActionPressed("ui_up")) direction -= Transform.Basis.Z;
			if (Input.IsActionPressed("ui_down")) direction += Transform.Basis.Z;
			if (Input.IsActionPressed("ui_left")) direction -= Transform.Basis.X;
			if (Input.IsActionPressed("ui_right")) direction += Transform.Basis.X;

 			// Prevent vertical movement
    		direction.Y = 0;

			if (direction != Vector3.Zero)
			{
				direction = direction.Normalized();
				velocity += direction * Acceleration * (float)delta;
				Vector2 horizontal_velocity = new(velocity.X, velocity.Z);
				float horizontal_speed = horizontal_velocity.Length();

				if (horizontal_speed > maxSpeed)
					horizontal_velocity = horizontal_velocity.Normalized() * maxSpeed;

				velocity.X = horizontal_velocity.X;
				velocity.Z = horizontal_velocity.Y;
			}
			else
			{
				// Dampen the movement when no input is made
				float dampingFactor = Mathf.Pow(Damping, (float)delta); // halves speed every second
				velocity.X *= dampingFactor;
				velocity.Z *= dampingFactor;

				if (new Vector2(velocity.X, velocity.Z).Length() < StopThreshold)
				{
					velocity.X = 0;
					velocity.Z = 0;
				}
			}
		}

		private void ApplyGravity(double delta)
		{
			velocity -= new Vector3(0, Gravity * (float)delta, 0);
		}
		
		private void UpdatePosition()
		{
			Position += velocity;
		}
	}
}
