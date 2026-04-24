using Godot;
using FarmGame.Scripts.UI;

namespace FarmGame.Scripts
{
	public partial class Main : Node3D
	{
		[Export] public float LookingAtDistance = 5.0f;
		[Export] public bool ShowHitIndicator = false;

		private Camera3D camera;
		private Debug debugPanel;
		private TileIndicator tileIndicator;
		private MeshInstance3D hitIndicator;

		public override void _Ready()
		{
			Engine.MaxFps = 0;
			camera = GetNode<Camera3D>("Player/Camera");
			debugPanel = (Debug)GetNode<CanvasLayer>("DebugUI");

			tileIndicator = new TileIndicator
			{
				Mesh = new PlaneMesh() { Size = Vector2.One },
				MaterialOverride = new ShaderMaterial() { Shader = GD.Load<Shader>("res://Shaders/tile_indicator.gdshader") }
			};
			AddChild(tileIndicator);

			if (ShowHitIndicator)
			{
				hitIndicator = new MeshInstance3D()
				{
					Mesh = new BoxMesh()
					{
						Size = Vector3.One * 0.1f
					},
					MaterialOverride = new StandardMaterial3D()
					{
						AlbedoColor = Colors.Red
					}
				};
				AddChild(hitIndicator);
			}
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
			// if (camera == null || field == null)
			// {
			tileIndicator.Hide();
			return;
			// }

			// PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
			// Vector2 center = GetViewport().GetVisibleRect().Size / 2;
			// Vector3 from = camera.ProjectRayOrigin(center);
			// Vector3 dir = camera.ProjectRayNormal(center);
			// Vector3 to = from + dir * LookingAtDistance;

			// var query = PhysicsRayQueryParameters3D.Create(from, to);
			// query.CollideWithAreas = false;
			// query.CollideWithBodies = true;

			// Dictionary hit = spaceState.IntersectRay(query);
			// if (hit.Count == 0 || !hit.TryGetValue("position", out Variant hitPositionVariant))
			// {
			// 	tileIndicator.Hide();
			// 	hitIndicator?.Hide();
			// 	return;
			// }

			// tileIndicator.Show();
			// hitIndicator?.Show();

			// Vector3 hitPosition = hitPositionVariant.AsVector3();
			// if (hitIndicator != null)
			// 	hitIndicator.Position = hitPosition;

			// Vector3 localHitPosition = field.ToLocal(hitPosition);
			// Vector2I gridPosition = field.WorldToGridPosition(localHitPosition);
			// Tile hoveredTile = field.GetTile(gridPosition);
			// float tileTop = hoveredTile != null ? hoveredTile.Height : 0f;
			// Vector3 TileIndicatorPosition = field.ToGlobal(field.GridToWorldPosition(gridPosition) + new Vector3(0, tileTop + 0.1f, 0));
			// tileIndicator.SetTargetPosition(TileIndicatorPosition);
			// debugPanel.LookingAt(hoveredTile);
		}
	}
}
