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

		private readonly Dictionary<Vector2I, Tile> tiles = [];
		private readonly Dictionary<TileType, MultiMeshInstance3D> tileRenderers = [];

		public override void _Ready()
		{
			CallDeferred(MethodName.InitializeField);
		}

		public void InitializeField()
		{
			InitializeField(TileSize);
		}

		public void InitializeField(float TileSize)
		{
			this.TileSize = TileSize;
			ClearField();

			// CreateUniformField(TileType.Grass);
			CreateRandomField();

			RenderTiles();
			BuildTerrainCollision();
		}

		private void CreateUniformField(TileType tileType)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Vector2I gridPos = new(x, y);
					Tile tile = TileFactory.CreateTile(tileType, gridPos);
					AddTile(gridPos, tile);
				}
			}
		}

		private void CreateRandomField()
		{
			var random = new RandomNumberGenerator();
			random.Randomize();

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Vector2I gridPos = new(x, y);
					TileType tileType = (TileType)(random.Randi() % 3);
					Tile tile = TileFactory.CreateTile(tileType, gridPos);
					AddTile(gridPos, tile);
				}
			}
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

			foreach (IGrouping<TileType, Tile> group in groupedTiles)
			{
				TileType tileType = group.Key;
				List<Tile> tilesOfType = [.. group];

				// Create material first (needed for mesh)
				Material material = CreateMaterial(tileType, GetTileColor(tilesOfType[0]));

				// Create mesh with material baked in
				Mesh mesh = CreateMesh(material, tilesOfType[0].Height);

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
					multiMesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, worldPos));

					if (i == 0)
					{
						GD.Print($"First {tileType} tile at grid {tile.GridPosition}, world pos: {worldPos}");
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
					Position = basePos + new Vector3(0, h, 0)
				};
				body.AddChild(shape);
			}
		}

		/// <summary>
		/// Create an ArrayMesh box with material applied
		/// </summary>
		private ArrayMesh CreateMesh(Material material, float height)
		{
			float halfSize = TileSize / 2f;
			SurfaceTool surfaceTool = new();
			surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
			surfaceTool.SetMaterial(material);

			Vector3 bottomFrontLeft = new(-halfSize, 0, -halfSize);
			Vector3 bottomFrontRight = new(halfSize, 0, -halfSize);
			Vector3 bottomBackRight = new(halfSize, 0, halfSize);
			Vector3 bottomBackLeft = new(-halfSize, 0, halfSize);
			Vector3 topFrontLeft = new(-halfSize, height, -halfSize);
			Vector3 topFrontRight = new(halfSize, height, -halfSize);
			Vector3 topBackRight = new(halfSize, height, halfSize);
			Vector3 topBackLeft = new(-halfSize, height, halfSize);

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

			GD.Print($"Added quad with vertices: {a}, {b}, {c}, {d} and normal {normal}");
		}

		/// <summary>
		/// Create a material for the given tile
		/// </summary>
		private static StandardMaterial3D CreateMaterial(TileType tileType, Color color)
		{
			// if (tileType == TileType.Grass)
			// {
			// 	var transparentGrass = new StandardMaterial3D
			// 	{
			// 		AlbedoColor = new Color(color.R, color.G, color.B, 0.35f),
			// 		Transparency = BaseMaterial3D.TransparencyEnum.Alpha
			// 	};
			// 	return transparentGrass;
			// }

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

		/// <summary>
		/// Get the world-space vertices of the first tile
		/// </summary>
		public Vector3[] GetFirstTileVertices()
		{
			if (tiles.Count == 0)
				return [];

			Tile firstTile = tiles.Values.First();
			float halfSize = TileSize / 2f;
			float height = firstTile.Height;
			Vector3 basePos = GridToWorldPosition(firstTile.GridPosition);
			// Apply the same transform as in RenderTiles
			Vector3 origin = basePos + new Vector3(0, height / 2, 0);
			Basis basis = Basis.Identity.Scaled(new Vector3(1.0f, height, 1.0f));

			// Local vertices of the mesh (Y from 0 to 1.0, then scaled by height in rendering)
			Vector3[] localVertices =
			[
				new Vector3(-halfSize, 0, -halfSize),      // bottomFrontLeft
				new Vector3(halfSize, 0, -halfSize),       // bottomFrontRight
				new Vector3(halfSize, 0, halfSize),        // bottomBackRight
				new Vector3(-halfSize, 0, halfSize),       // bottomBackLeft
				new Vector3(-halfSize, 1.0f, -halfSize),   // topFrontLeft
				new Vector3(halfSize, 1.0f, -halfSize),    // topFrontRight
				new Vector3(halfSize, 1.0f, halfSize),     // topBackRight
				new Vector3(-halfSize, 1.0f, halfSize)     // topBackLeft
			];

			// Transform to world space: world = origin + basis * local
			Vector3[] worldVertices = new Vector3[8];
			for (int i = 0; i < localVertices.Length; i++)
			{
				worldVertices[i] = origin + basis * localVertices[i];
			}

			return worldVertices;
		}
	}
}
