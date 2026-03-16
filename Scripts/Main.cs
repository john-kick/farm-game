using Godot;
using FarmGame.Scripts.UI;
using Godot.Collections;
using FarmGame.Scripts.Tiles;

namespace FarmGame.Scripts
{
	public partial class Main : Node3D
	{
		[Export] public Vector2I Size;
		[Export] public Node3D Field;
		[Export] public Mesh.PrimitiveType PrimitiveType = Mesh.PrimitiveType.Triangles;

		private Tile[] tiles;
		private Camera3D camera;

		private TileIndicator tileIndicator;

		public override void _Ready()
		{
			camera = GetNode<Camera3D>("Player/Camera");
			tileIndicator = (TileIndicator)GetNode<Node3D>("TileIndicator");

			if (Field == null)
			{
				Field = new Node3D();
				AddChild(Field);
			}

			// GenerateRandomField();
			GenerateUniformField(TileType.GRASS);
			// GenerateTestField();
			InitRender();
			// GrassTile grassTile = GetNode<GrassTile>("Tiles/GrassTile");
			// grassTile.Render([]);
		}

		public override void _Process(double delta)
		{
			CheckCollision();
		}

		public override void _PhysicsProcess(double delta)
		{
		}

		/// <summary>
		/// Generates random tiles and stores them in the `Tiles` array
		/// </summary>
		private void GenerateRandomField()
		{
			tiles = new Tile[Size.X * Size.Y];

			for (int z = 0; z < Size.Y; z++)
			{
				for (int x = 0; x < Size.X; x++)
				{
					Tile tile = TileFactory.GetRandomTile();
					tile.GridPosition = new Vector2I(x, z);
					tiles[z * Size.X + x] = tile;
				}
			}
		}

		private void GenerateUniformField(TileType type)
		{
			tiles = new Tile[Size.X * Size.Y];

			for (int z = 0; z < Size.Y; z++)
			{
				for (int x = 0; x < Size.X; x++)
				{
					Tile tile = TileFactory.GetTile(type);
					tile.GridPosition = new Vector2I(x, z);
					tiles[z * Size.X + x] = tile;
				}
			}
		}

		private void GenerateTestField()
		{
			Size = new Vector2I(2, 2);
			tiles = [
				new StoneTile() {GridPosition = new Vector2I(0,0)}, // top-left
			new DirtTile()  {GridPosition = new Vector2I(1,0)}, // top-right
			new DirtTile()  {GridPosition = new Vector2I(0,1)}, // bottom-left
			new StoneTile() {GridPosition = new Vector2I(1,1)}  // bottom-right
			];
		}

		/// <summary>
		/// Adds the tiles contained in the `Tiles` array to the Scene
		/// </summary>
		private void InitRender()
		{
			for (int z = 0; z < Size.Y; z++)
			{
				for (int x = 0; x < Size.X; x++)
				{
					Tile tile = GetTile(x, z);
					Neighbor<Tile>[] neighbors = GetNeighbors(tile);
					Field.AddChild(tile);
					tile.Render(neighbors, PrimitiveType);
				}
			}
		}

		private void ReplaceTile(Tile oldTile, TileType newType)
		{
			Vector2I gridPosition = oldTile.GridPosition;

			// Generate the new tile
			Tile newTile = TileFactory.GetTile(newType);
			newTile.GridPosition = gridPosition;

			// Add the new tile to the tiles list
			tiles[gridPosition.Y * Size.X + gridPosition.X] = newTile;

			// Remove the old tile from the field
			Field.RemoveChild(oldTile);
			oldTile.QueueFree();

			// Add the new tile to the field
			Field.AddChild(newTile);
			Neighbor<Tile>[] neighbors = GetNeighbors(newTile);
			newTile.Render(neighbors);

			// Re-render the neighbors
			foreach (Neighbor<Tile> n in neighbors)
			{
				n.Element.Render(GetNeighbors(n.Element));
			}
		}

		private Tile GetTile(int x, int z)
		{
			if (x < 0 || z < 0 || x >= Size.X || z >= Size.Y)
				return TileFactory.GetTile(TileType.BASE);

			return tiles[z * Size.X + x];
		}

		private void CheckCollision()
		{
			var spaceState = GetWorld3D().DirectSpaceState;

			Vector2 center = GetViewport().GetVisibleRect().Size / 2;

			Vector3 from = camera.ProjectRayOrigin(center);
			Vector3 dir = camera.ProjectRayNormal(center);
			Vector3 to = from + dir * 1000f;

			var query = PhysicsRayQueryParameters3D.Create(from, to);
			Dictionary collisions = spaceState.IntersectRay(query);

			if (collisions.Count > 0)
			{
				HandleCollision(collisions);
			}
			else
			{
				tileIndicator.FHide();
			}
		}

		private void HandleCollision(Dictionary collisions)
		{
			GodotObject collider = (GodotObject)collisions["collider"];

			if (collider is Tile tile)
			{
				tileIndicator.Show(tile);
				if (Input.IsActionJustPressed("ui_primary_action"))
				{

					if (tile.GetTileType() == TileType.GRASS)
					{
						ReplaceTile(tile, TileType.DIRT);
					}
					else if (tile.GetTileType() == TileType.DIRT)
					{
						ReplaceTile(tile, TileType.STONE);
					}
					else if (tile.GetTileType() == TileType.STONE)
					{
						ReplaceTile(tile, TileType.GRASS);
					}
				}
			}
			else
			{
				tileIndicator.FHide();
			}
		}

		private Neighbor<Tile>[] GetNeighbors(Tile tile)
		{
			int x = tile.GridPosition.X;
			int z = tile.GridPosition.Y;

			return [
				new Neighbor<Tile>()
			{
				// Right
				Element = GetTile(x + 1, z),
				Offset = new Vector2I(1, 0)
			},
			new Neighbor<Tile>()
			{
				// Left
				Element = GetTile(x - 1, z),
				Offset = new Vector2I(-1, 0)
			},
			new Neighbor<Tile>()
			{
				// Top
				Element = GetTile(x, z - 1),
				Offset = new Vector2I(0, -1)
			},
			new Neighbor<Tile>()
			{
				// Bottom
				Element = GetTile(x,z + 1),
				Offset = new Vector2I(0, 1)
			}
			];
		}
	}
}
