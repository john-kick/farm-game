using Godot;
using System.Collections.Generic;
using System.Linq;

namespace FarmGame.Scripts.Tiles
{
	public partial class TileField : Node3D
	{
		private const string TerrainCollisionName = "TerrainCollision";

		[Export] public int Width = 10;
		[Export] public int Height = 10;
		[Export] public float TileSize = 1.0f;

		private Dictionary<Vector2I, Tile> tiles = new();
		private Dictionary<TileType, MultiMeshInstance3D> tileRenderers = new();

		public override void _Ready()
		{
			CallDeferred(MethodName.InitializeField);
		}

		/// <summary>
		/// Initialize the tile field with random tiles
		/// </summary>
		public void InitializeField()
		{
			ClearField();
			var random = new RandomNumberGenerator();
			random.Randomize();

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Vector2I gridPos = new Vector2I(x, y);
					TileType tileType = (TileType)(random.Randi() % 3);
					Tile tile = TileFactory.CreateTile(tileType, gridPos);
					AddTile(gridPos, tile);
				}
			}

			GD.Print($"Initialized {tiles.Count} tiles");
			RenderTiles();
			BuildTerrainCollision();
			GD.Print($"Rendered {tileRenderers.Count} tile groups");
		}

		/// <summary>
		/// Add a tile to the field at the specified grid position
		/// </summary>
		public void AddTile(Vector2I gridPos, Tile tile)
		{
			if (tiles.ContainsKey(gridPos))
			{
				RemoveTile(gridPos);
			}

			tile.GridPosition = gridPos;
			tiles[gridPos] = tile;
		}

		/// <summary>
		/// Remove a tile from the field
		/// </summary>
		public void RemoveTile(Vector2I gridPos)
		{
			tiles.Remove(gridPos);
		}

		/// <summary>
		/// Get a tile at the specified grid position
		/// </summary>
		public Tile GetTile(Vector2I gridPos)
		{
			tiles.TryGetValue(gridPos, out Tile tile);
			return tile;
		}

		/// <summary>
		/// Convert grid coordinates to world position
		/// </summary>
		public Vector3 GridToWorldPosition(Vector2I gridPos)
		{
			return new Vector3(gridPos.X * TileSize, 0, gridPos.Y * TileSize);
		}

		/// <summary>
		/// Convert world position to grid coordinates
		/// </summary>
		public Vector2I WorldToGridPosition(Vector3 worldPos)
		{
			return new Vector2I(
				Mathf.RoundToInt(worldPos.X / TileSize),
				Mathf.RoundToInt(worldPos.Z / TileSize)
			);
		}

		/// <summary>
		/// Clear all tiles from the field
		/// </summary>
		public void ClearField()
		{
			tiles.Clear();

			// Remove old renderers
			foreach (var renderer in tileRenderers.Values)
			{
				renderer.QueueFree();
			}
			tileRenderers.Clear();

			GetNodeOrNull<StaticBody3D>(TerrainCollisionName)?.QueueFree();
		}

		/// <summary>
		/// Check if a grid position is within field bounds
		/// </summary>
		public bool IsWithinBounds(Vector2I gridPos)
		{
			return gridPos.X >= 0 && gridPos.X < Width &&
				   gridPos.Y >= 0 && gridPos.Y < Height;
		}

		/// <summary>
		/// Render all tiles using MultiMesh for performance
		/// </summary>
		private void RenderTiles()
		{
			// Group tiles by type
			var groupedTiles = tiles.Values.GroupBy(t => t.TileType).ToList();

			foreach (var group in groupedTiles)
			{
				TileType tileType = group.Key;
				List<Tile> tilesOfType = [.. group];

				// Create material first (needed for mesh)
				Material material = CreateMaterial(GetTileColor(tilesOfType[0]));

				// Create mesh with material baked in
				Mesh mesh = CreateMesh(material);

				MultiMesh multiMesh = new()
				{
					Mesh = mesh,
					TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
					InstanceCount = tilesOfType.Count
				};

				// Set transforms for each tile
				for (int i = 0; i < tilesOfType.Count; i++)
				{
					Tile tile = tilesOfType[i];
					Vector3 worldPos = GridToWorldPosition(tile.GridPosition);
					float height = tile.Height;

					// Struct default Basis is zero; must start from Identity before scaling.
					Basis basis = Basis.Identity.Scaled(new Vector3(1.0f, height, 1.0f));
					Vector3 origin = worldPos + new Vector3(0, height / 2, 0);
					multiMesh.SetInstanceTransform(i, new Transform3D(basis, origin));

					if (i == 0)
					{
						GD.Print($"First {tileType} tile at grid {tile.GridPosition}, world pos: {worldPos}, instance origin: {origin}");
					}
				}

				// Create MultiMeshInstance3D for this tile type
				MultiMeshInstance3D meshInstance = new()
				{
					Multimesh = multiMesh,
					MaterialOverride = material,
					Name = $"{tileType}Group",
					Visible = true,
					CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
				};
				AddChild(meshInstance);

				GD.Print($"Created {tileType}Group with {tilesOfType.Count} instances. Child count in TileField: {GetChildCount()}");

				tileRenderers[tileType] = meshInstance;
			}
		}

		/// <summary>
		/// Static collision aligned with each tile box (same center and size as the multimesh instances).
		/// </summary>
		private void BuildTerrainCollision()
		{
			var body = new StaticBody3D { Name = TerrainCollisionName };
			AddChild(body);

			foreach (Tile tile in tiles.Values)
			{
				float h = tile.Height;
				Vector3 basePos = GridToWorldPosition(tile.GridPosition);
				var box = new BoxShape3D { Size = new Vector3(TileSize, h, TileSize) };
				var shape = new CollisionShape3D
				{
					Shape = box,
					Position = basePos + new Vector3(0, h / 2f, 0)
				};
				body.AddChild(shape);
			}
		}

		/// <summary>
		/// Create an ArrayMesh box with material applied
		/// </summary>
		private Mesh CreateMesh(Material material)
		{
			float halfSize = TileSize / 2f;
			SurfaceTool surfaceTool = new();
			surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
			surfaceTool.SetMaterial(material);

			Vector3 bottomFrontLeft = new(-halfSize, 0, -halfSize);
			Vector3 bottomFrontRight = new(halfSize, 0, -halfSize);
			Vector3 bottomBackRight = new(halfSize, 0, halfSize);
			Vector3 bottomBackLeft = new(-halfSize, 0, halfSize);
			Vector3 topFrontLeft = new(-halfSize, 1.0f, -halfSize);
			Vector3 topFrontRight = new(halfSize, 1.0f, -halfSize);
			Vector3 topBackRight = new(halfSize, 1.0f, halfSize);
			Vector3 topBackLeft = new(-halfSize, 1.0f, halfSize);

			AddQuad(surfaceTool, topFrontLeft, topFrontRight, topBackRight, topBackLeft, Vector3.Up);
			AddQuad(surfaceTool, bottomFrontLeft, bottomFrontRight, topFrontRight, topFrontLeft, Vector3.Forward);
			AddQuad(surfaceTool, bottomBackRight, bottomBackLeft, topBackLeft, topBackRight, Vector3.Back);
			AddQuad(surfaceTool, bottomBackLeft, bottomFrontLeft, topFrontLeft, topBackLeft, Vector3.Left);
			AddQuad(surfaceTool, bottomFrontRight, bottomBackRight, topBackRight, topFrontRight, Vector3.Right);

			return surfaceTool.Commit();
		}

		private static void AddQuad(SurfaceTool surfaceTool, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal)
		{
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(0, 0));
			surfaceTool.AddVertex(a);
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(1, 0));
			surfaceTool.AddVertex(b);
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(1, 1));
			surfaceTool.AddVertex(c);

			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(0, 0));
			surfaceTool.AddVertex(a);
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(1, 1));
			surfaceTool.AddVertex(c);
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(0, 1));
			surfaceTool.AddVertex(d);
		}

		/// <summary>
		/// Create a material for the given tile
		/// </summary>
		private Material CreateMaterial(Color color)
		{
			var material = new StandardMaterial3D
			{
				AlbedoColor = color
			};
			return material;
		}

		/// <summary>
		/// Get the color of a tile
		/// </summary>
		private static Color GetTileColor(Tile tile)
		{
			return tile switch
			{
				GrassTile g => g.GrassColor,
				DirtTile d => d.DirtColor,
				StoneTile s => s.StoneColor,
				_ => Colors.White
			};
		}
	}
}
