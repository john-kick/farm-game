using System;
using Godot;

namespace FarmGame.Scripts.UI
{
	public partial class Debug : CanvasLayer
	{
		[Export] public Node3D player;
		private Label posLabel;
		private Label fpsLabel;

		public override void _Ready()
		{
			posLabel = GetNode<Label>("Panel/VFlowContainer/PlayerPosition");
			fpsLabel = GetNode<Label>("Panel/VFlowContainer/FPS");
		}

		public override void _Process(double delta)
		{
			Vector3 pos = player.GlobalTransform.Origin;
			posLabel.Text = $"Player Position: X: {Math.Round(pos.X, 2)}  Y: {Math.Round(pos.Y, 2)}  Z: {Math.Round(pos.Z, 2)}";
			fpsLabel.Text = $"FPS {Engine.GetFramesPerSecond()}";
		}
	}
}
