using Godot;
using FarmGame.Scripts.UI;
using FarmGame.Scripts.Tiles;

namespace FarmGame.Scripts
{
	public partial class Main : Node3D
	{
		[Export] public float LookingAtDistance = 100.0f;

		private Camera3D camera;
		private CharacterBody3D player;
		private TileField tileField;
		private Debug debugPanel;
		private TileIndicator tileIndicator;

		public override void _Ready()
		{
			Engine.MaxFps = 0;
			camera = GetNode<Camera3D>("Player/Camera");
			player = GetNode<CharacterBody3D>("Player");
			tileField = GetNode<TileField>("TileField");
			debugPanel = (Debug)GetNode<CanvasLayer>("DebugUI");
			tileIndicator = new TileIndicator
			{
				Mesh = new PlaneMesh() { Size = new Vector2(tileField.TileSize, tileField.TileSize) },
				MaterialOverride = new ShaderMaterial() { Shader = GD.Load<Shader>("res://Shaders/tile_indicator.gdshader") }
			};
			AddChild(tileIndicator);
		}

		public override void _Process(double delta)
		{
			CheckInput();
			LookingAt();
		}

		private void CheckInput()
		{
			if (Input.IsActionJustPressed("ui_debug"))
			{
				debugPanel.Visible = !debugPanel.Visible;
			}
		}

		private void LookingAt()
		{
			if (camera == null || tileField == null)
			{
				tileIndicator.Hide();
				return;
			}

			Vector3 rayStart = camera.GlobalTransform.Origin;
			Vector3 rayDirection = -camera.GlobalTransform.Basis.Z;
			Vector3 rayEnd = rayStart + rayDirection * LookingAtDistance;

			var query = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd);
			query.CollideWithAreas = false;
			query.CollideWithBodies = true;

			var hit = GetWorld3D().DirectSpaceState.IntersectRay(query);
			if (hit.Count == 0 || !hit.TryGetValue("position", out Variant hitPositionVariant))
			{
				tileIndicator.Hide();
				return;
			}

			tileIndicator.Show();

			Vector3 hitPosition = hitPositionVariant.AsVector3();
			Vector3 localHitPosition = tileField.ToLocal(hitPosition);
			Vector2I gridPosition = tileField.WorldToGridPosition(localHitPosition);
			Tile hoveredTile = tileField.GetTile(gridPosition);
			float tileTop = hoveredTile != null ? hoveredTile.Height : 0f;
			Vector3 TileIndicatorPosition = tileField.ToGlobal(tileField.GridToWorldPosition(gridPosition) + new Vector3(0, tileTop + 0.1f, 0));
			tileIndicator.SetTargetPosition(TileIndicatorPosition);
			debugPanel.LookingAt(hoveredTile);
		}
	}
}
