using Godot;
using System.Collections.Generic;
using System.Linq;

namespace FarmGame.Scripts.Tiles
{
	public partial class Field : Node3D
	{
		private const string TerrainCollisionName = "TerrainCollision";

		[Export] public int Width = 20;
		[Export] public int Height = 20;
		[Export] public float TileSize = 1.0f;

		private readonly Dictionary<Vector2I, Tile> tiles = [];
		private readonly Dictionary<TileType, MultiMeshInstance3D> tileRenderers = [];

		public override void _Ready()
		{
			InitializeField(TileSize);
		}

		public void InitializeField(float TileSize)
		{
			this.TileSize = TileSize;
			ClearField();
			CreateRandomField();
			RenderTiles();
		}

		private void CreateUniformField(TileType tileType)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Vector2I gridPos = new(x, y);
					Tile tile = TileFactory.CreateTile(tileType);
					tile.GridPosition = gridPos;
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
					Tile tile = TileFactory.CreateTile(tileType);
					tile.GridPosition = gridPos;
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
				RemoveTile(gridPos);
			
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
				renderer.QueueFree();
			
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
			StaticBody3D body = new() { Name = TerrainCollisionName };
			List<IGrouping<TileType, Tile>> groupedTiles = [.. tiles.Values.GroupBy(t => t.TileType)];

			foreach (IGrouping<TileType, Tile> group in groupedTiles)
				RenderTileGroup(group, body);

			AddChild(body);
		}

		private void RenderTileGroup(IGrouping<TileType, Tile> group, StaticBody3D body)
		{
			TileType tileType = group.Key;
			List<Tile> tilesOfType = [.. group];

			ArrayMesh mesh = tilesOfType[0].CreateMesh(TileSize);
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
				SetTileTransform(multiMesh, worldPos, tile, i);
				AddCollisionShape(body, worldPos, mesh);
			}

			// Create MultiMeshInstance3D for this tile type
			MultiMeshInstance3D meshInstance = new()
			{
				Multimesh = multiMesh,
				Name = $"{tileType}Group",
				Visible = true,
				CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
			};
			AddChild(meshInstance);

			tileRenderers[tileType] = meshInstance;
		}

		private static void SetTileTransform(MultiMesh multiMesh, Vector3 position, Tile tile, int index)
		{
			multiMesh.SetInstanceTransform(index, new Transform3D(Basis.Identity, position));
		}

		private static void AddCollisionShape(StaticBody3D body, Vector3 position, ArrayMesh mesh)
		{
			CollisionShape3D collisionShape = new()
			{
				Shape = mesh.CreateTrimeshShape(),
				Position = position
			};
			body.AddChild(collisionShape);
		}
	}
}
