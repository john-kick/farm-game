using Godot;
using FarmGame.Scripts.UI;
using Godot.Collections;

namespace FarmGame.Scripts
{
	public partial class Main : Node3D
	{
		private Camera3D camera;
		private Debug debugPanel;

		public override void _Ready()
		{
			Engine.MaxFps = 0;
			camera = GetNode<Camera3D>("Player/Camera");
			debugPanel = (Debug)GetNode<CanvasLayer>("DebugUI");
		}

		public override void _Process(double delta)
		{
			CheckInput();
		}

		private void CheckInput()
		{
			if (Input.IsActionJustPressed("ui_debug"))
			{
				debugPanel.Visible = !debugPanel.Visible;
			}

		}
	}
}
