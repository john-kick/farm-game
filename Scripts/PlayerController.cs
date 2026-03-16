using Godot;

public partial class PlayerController : CharacterBody3D
{
	[Export] public float MoveSpeed = 20f;
	[Export] public float MouseSensitivity = 0.15f;
	[Export] public float FastMultiplier = 3f;
	[Export] public float Gravity = 9.8f;
	[Export] public float JumpForce = 10f;

	private Camera3D camera;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		camera = GetNode<Camera3D>("Camera");
		camera.Current = true;
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
			// Horizontal rotation on the parent node
			RotationDegrees -= new Vector3(0, mouseMotion.Relative.X * MouseSensitivity, 0);

			// Vertical rotation on the camera
			float newPitch = camera.RotationDegrees.X - mouseMotion.Relative.Y * MouseSensitivity;
			newPitch = Mathf.Clamp(newPitch, -90, 90); // Prevent flipping
			camera.RotationDegrees = new Vector3(newPitch, camera.RotationDegrees.Y, camera.RotationDegrees.Z);
		}
	}

	public override void _Process(double delta)
	{
		float speed = MoveSpeed;

		if (Input.IsActionPressed("ui_shift"))
			speed *= FastMultiplier;

		Vector3 direction = Vector3.Zero;

		if (Input.IsActionPressed("ui_up")) direction -= Transform.Basis.Z;
		if (Input.IsActionPressed("ui_down")) direction += Transform.Basis.Z;
		if (Input.IsActionPressed("ui_left")) direction -= Transform.Basis.X;
		if (Input.IsActionPressed("ui_right")) direction += Transform.Basis.X;
		if (Input.IsActionPressed("ui_page_up")) direction += Transform.Basis.Y;
		if (Input.IsActionPressed("ui_page_down")) direction -= Transform.Basis.Y;

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			Position += direction * speed * (float)delta;
		}

		if (!IsOnFloor())
		{
			Velocity += Vector3.Down * Gravity * (float)delta;
		}
		else
		{
			if (Input.IsActionJustPressed("ui_jump"))
			{
				Velocity = new Vector3(Velocity.X, JumpForce, Velocity.Z);
			}
			else
			{
				Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
			}
		}

		MoveAndSlide();
	}
}
