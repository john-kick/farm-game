using Godot;

namespace Scripts.Tests
{
    public partial class RayTestCamera : Camera3D
    {
		[Export] public float MouseSensitivity = 0.15f;

		public override void _Ready()
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
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
    }
}
