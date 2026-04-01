using System;
using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.UI
{
	public partial class Debug : CanvasLayer
	{
		[Export] public Node3D player;
		private Label posLabel;
		private Label fpsLabel;
		private Label lookingAtLabel;
		private Label verticesLabel;

		public override void _Ready()
		{
			fpsLabel = GetNode<Label>("Panel/VFlowContainer/FPS");
			posLabel = GetNode<Label>("Panel/VFlowContainer/PlayerInfo/PlayerPosition");
			lookingAtLabel = GetNode<Label>("Panel/VFlowContainer/EnvironmentInfo/LookingAt");
		}

		public override void _Process(double delta)
		{
			Vector3 pos = player.GlobalTransform.Origin;
			posLabel.Text = $"Player Position: X: {Math.Round(pos.X, 2)}  Y: {Math.Round(pos.Y, 2)}  Z: {Math.Round(pos.Z, 2)}";
			fpsLabel.Text = $"FPS {Engine.GetFramesPerSecond()}";
		}

		public void LookingAt(Tile tile)
		{
			lookingAtLabel.Text = $"Tile: {tile.TileType} {tile.GridPosition}, height: {tile.Height}";
		}
	}
}
