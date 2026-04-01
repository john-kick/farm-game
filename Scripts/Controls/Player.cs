using Godot;

namespace FarmGame.Scripts.Controls
{
	public partial class Player : CharacterBody3D
	{
		[Export] public float Speed = 1.0f;
		[Export] public float FastMultiplier = 2.0f;
		[Export] public float JumpStrength = 10.0f;
		[Export] public float MouseSensitivity = .15f;
		[Export] public float Gravity = 0.3f;
		[Export] public float DoubleTapTime = 0.2f;

		private float jumpLastPressed = 0.0f;
		private Vector3 targetVelocity;
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

		public override void _PhysicsProcess(double delta)
		{
			ApplyMovementInput(delta);
			ApplyGravity(delta);
			Move();
		}

		private void ApplyMovementInput(double delta)
		{
			float speed = Speed;

			if (Input.IsActionPressed("ui_shift"))
				speed *= FastMultiplier;

			Vector3 direction = Vector3.Zero;
			if (Input.IsActionPressed("ui_up")) direction -= Transform.Basis.Z;
			if (Input.IsActionPressed("ui_down")) direction += Transform.Basis.Z;
			if (Input.IsActionPressed("ui_left")) direction -= Transform.Basis.X;
			if (Input.IsActionPressed("ui_right")) direction += Transform.Basis.X;

			if (direction != Vector3.Zero)
			{
				direction.Y = 0;
				direction = direction.Normalized();
			}

			targetVelocity.X = direction.X * speed;
			targetVelocity.Z = direction.Z * speed;

			if (Input.IsActionJustPressed("ui_jump"))
			{
				if (IsOnFloor())
				{
					Jump();
				}
			}

			Velocity = targetVelocity;
		}

		private void Jump()
		{
			targetVelocity.Y = JumpStrength;
		}

		private void ApplyGravity(double delta)
		{
			if (!IsOnFloor()) targetVelocity.Y -= Gravity * (float)delta;
		}

		private void Move()
		{
			MoveAndSlide();
		}
	}
}
