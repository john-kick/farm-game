using Godot;
using FarmGame.Scripts.UI;
using Godot.Collections;
using FarmGame.Scripts.Tiles;

namespace FarmGame.Scripts
{
	public partial class Main : Node3D
	{
		[Export] public Vector2I Size;
		[Export] public Mesh.PrimitiveType PrimitiveType = Mesh.PrimitiveType.Triangles;

		private Field Field;
		private Camera3D camera;
		private TileIndicator tileIndicator;
		private Debug debugPanel;

		public override void _Ready()
		{
			Engine.MaxFps = 0;
			camera = GetNode<Camera3D>("Player/Camera");
			tileIndicator = (TileIndicator)GetNode<Node3D>("TileIndicator");
			debugPanel = (Debug)GetNode<CanvasLayer>("DebugUI");

			Field = new Field(Size, PrimitiveType);
			AddChild(Field);
		}

		public override void _Process(double delta)
		{
			CheckInput();
			CheckCollision();
		}

		private void CheckInput()
		{
			if (Input.IsActionJustPressed("ui_debug"))
			{
				debugPanel.Visible = !debugPanel.Visible;
			}

		}

		private void CheckCollision()
		{
			var spaceState = GetWorld3D().DirectSpaceState;

			Vector2 center = GetViewport().GetVisibleRect().Size / 2;

			Vector3 from = camera.ProjectRayOrigin(center);
			Vector3 dir = camera.ProjectRayNormal(center);
			Vector3 to = from + dir * 5f;

			var query = PhysicsRayQueryParameters3D.Create(from, to);
			Dictionary collisions = spaceState.IntersectRay(query);

			if (collisions.Count > 0)
			{
				HandleCollision(collisions);
			}
			else
			{
				tileIndicator.Hide();
			}
		}

		private void HandleCollision(Dictionary collisions)
		{
			GodotObject collider = (GodotObject)collisions["collider"];

			if (collider is Tile tile)
			{
				tileIndicator.SetTargetTile(tile);			
				tileIndicator.Show();			
				if (Input.IsActionJustPressed("ui_primary_action"))
				{

					if (tile.GetTileType() == TileType.GRASS)
					{
						Field.ReplaceTile(tile, TileType.DIRT);
					}
					else if (tile.GetTileType() == TileType.DIRT)
					{
						Field.ReplaceTile(tile, TileType.STONE);
					}
					else if (tile.GetTileType() == TileType.STONE)
					{
						Field.ReplaceTile(tile, TileType.GRASS);
					}
				}
			}
			else
			{
				tileIndicator.Hide();
			}
		}
	}
}
